using System.ComponentModel;
using StockExchange.Client.WinForms.Controls;
using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Models;
using StockExchange.Shared.DTOs;
using StockExchange.Shared.Models;

namespace StockExchange.Client.WinForms.Forms;

public partial class MainForm : Form
{
    private Control BuildAdminDashboard()
    {
        var page = AppTheme.CreatePage(2);
        page.AutoScroll = true;
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 170));
        page.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var stats = new EqualColumnPanel
        {
            Dock = DockStyle.Fill,
            Columns = 4,
            Gap = 12,
            Padding = new Padding(0, 0, 0, 12)
        };
        AddLoadingStats();
        page.Controls.Add(stats, 0, 0);

        var grid = AppTheme.CreateGrid();
        grid.DataSource = new BindingList<UserProfileDto>();
        page.Controls.Add(WrapGrid(grid, "Người dùng gần đây từ server"), 0, 1);

        async Task LoadDashboardAsync()
        {
            try
            {
                var dashboard = await _authService.AdminGetDashboardAsync();
                stats.Controls.Clear();
                stats.Controls.Add(BuildStatCard("Người dùng", dashboard.TotalUsers.ToString("N0"), "Tổng user trong database", AppTheme.Primary));
                stats.Controls.Add(BuildStatCard("Cổ phiếu hoạt động", dashboard.ActiveStocks.ToString("N0"), "Stock có IsActive = true", AppTheme.Success));
                stats.Controls.Add(BuildStatCard("Cấu hình simulation", dashboard.SimulationConfigs.ToString("N0"), "Dòng trong stock_simulations", AppTheme.Warning));
                stats.Controls.Add(BuildStatCard("Client kết nối", dashboard.ConnectedClients.ToString("N0"), "Phiên TCP đang hoạt động", Color.FromArgb(124, 58, 237)));
                grid.DataSource = new BindingList<UserProfileDto>(dashboard.RecentUsers);
            }
            catch (Exception ex)
            {
                AddErrorStats();
                MessageBox.Show(this, ex.Message, "Không thể tải dashboard admin",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void AddLoadingStats()
        {
            stats.Controls.Clear();
            stats.Controls.Add(BuildStatCard("Người dùng", "...", "Đang tải từ server", AppTheme.Primary));
            stats.Controls.Add(BuildStatCard("Cổ phiếu hoạt động", "...", "Đang tải từ server", AppTheme.Success));
            stats.Controls.Add(BuildStatCard("Cấu hình simulation", "...", "Đang tải từ server", AppTheme.Warning));
            stats.Controls.Add(BuildStatCard("Client kết nối", "...", "Đang tải từ server", Color.FromArgb(124, 58, 237)));
        }

        void AddErrorStats()
        {
            stats.Controls.Clear();
            stats.Controls.Add(BuildStatCard("Người dùng", "-", "Tải thất bại", AppTheme.Danger));
            stats.Controls.Add(BuildStatCard("Cổ phiếu hoạt động", "-", "Tải thất bại", AppTheme.Danger));
            stats.Controls.Add(BuildStatCard("Cấu hình simulation", "-", "Tải thất bại", AppTheme.Danger));
            stats.Controls.Add(BuildStatCard("Client kết nối", "-", "Tải thất bại", AppTheme.Danger));
        }

        page.HandleCreated += (_, _) => _ = LoadDashboardAsync();
        return page;
    }
    private Control BuildUsersPage()
    {
        var page = AppTheme.CreatePage(2);
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        page.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var toolbar = BuildToolbar("Tìm username hoặc email...", "Thêm người dùng", out var search, out var add);
        page.Controls.Add(toolbar, 0, 0);

        var grid = AppTheme.CreateGrid();
        var users = new List<UserProfileDto>();

        void Bind(string keyword = "")
        {
            var filtered = users
                .Where(user => string.IsNullOrWhiteSpace(keyword)
                    || user.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || user.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || user.Role.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            grid.DataSource = new BindingList<UserProfileDto>(filtered);
        }

        async Task LoadUsersAsync()
        {
            try
            {
                users = await _authService.AdminGetUsersAsync();
                Bind(search.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Lỗi tải người dùng",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        page.HandleCreated += (_, _) => _ = LoadUsersAsync();

        search.TextChanged += (_, _) => Bind(search.Text);

        add.Click += async (_, _) =>
        {
            using var dialog = new UserEditorForm();

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                await _authService.AdminCreateUserAsync(dialog.CreateRequest);
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Lỗi thêm người dùng",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        grid.CellDoubleClick += async (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not UserProfileDto user)
                return;

            using var dialog = new UserEditorForm(user);

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                await _authService.AdminUpdateUserAsync(dialog.UpdateRequest);
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Lỗi cập nhật người dùng",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        var card = WrapGrid(grid, "Danh sách người dùng từ server • Nhấp đúp để chỉnh sửa");
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 68,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 6, 8, 6)
        };
        var delete = AppTheme.CreateButton("Xóa", false);
        delete.ForeColor = AppTheme.Danger;
        delete.Click += async (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not UserProfileDto user)
                return;

            if (user.UserId == _profile.UserId)
            {
                MessageBox.Show(this, "Không thể xóa tài khoản admin đang đăng nhập.",
                    "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmation = MessageBox.Show(
                this,
                $"Xóa user {user.Username} khỏi database?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmation != DialogResult.Yes)
                return;

            try
            {
                await _authService.AdminDeleteUserAsync(user.UserId);
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Lỗi xóa người dùng",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
        actions.Controls.Add(delete);
        card.Controls.Add(actions);
        page.Controls.Add(card, 0, 1);
        return page;
    }

    private Control BuildAdminStocksPage()
    {
        var page = AppTheme.CreatePage(2);
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        page.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var toolbar = BuildToolbar("Tìm mã, công ty hoặc ngành...", "Thêm stock", out var search, out var add);
        page.Controls.Add(toolbar, 0, 0);

        var grid = AppTheme.CreateGrid();
        var stocks = new BindingList<StockRow>();
        void Bind(string keyword = "")
        {
            grid.DataSource = new BindingList<StockRow>(stocks
                .Where(stock => string.IsNullOrWhiteSpace(keyword)
                    || stock.Symbol.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || stock.Company.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || stock.Sector.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList());
        }

        async Task LoadStocksAsync()
        {
            try
            {
                var databaseStocks = await _stockService.GetAllAsync();
                stocks = new BindingList<StockRow>(databaseStocks.Select(ToStockRow).ToList());
                Bind(search.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Không thể tải cổ phiếu",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        search.TextChanged += (_, _) => Bind(search.Text);
        add.Click += async (_, _) =>
        {
            using var dialog = new StockEditorForm();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var created = await _stockService.CreateAsync(ToUpdateDto(dialog.Result));
                    stocks.Add(ToStockRow(created));
                    Bind(search.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Không thể thêm cổ phiếu",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        };

        var card = WrapGrid(grid, "Quản lý cổ phiếu");
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 68,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 6, 8, 6)
        };
        var delete = AppTheme.CreateButton("Xóa", false);
        delete.ForeColor = AppTheme.Danger;
        delete.Click += async (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not StockRow stock) return;
            var confirmation = MessageBox.Show(
                this,
                $"Xóa cổ phiếu {stock.Symbol} khỏi database?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirmation != DialogResult.Yes) return;

            try
            {
                await _stockService.DeleteAsync(stock.Id);
                stocks.Remove(stocks.First(item => item.Id == stock.Id));
                Bind(search.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Không thể xóa cổ phiếu",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
        var edit = AppTheme.CreateButton("Chỉnh sửa");
        async Task EditSelectedStockAsync()
        {
            if (grid.CurrentRow?.DataBoundItem is not StockRow stock) return;

            using var dialog = new StockEditorForm(stock);
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var updated = await _stockService.UpdateAsync(ToUpdateDto(dialog.Result));
                ApplyStock(stock, updated);
                Bind(search.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Không thể cập nhật cổ phiếu",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        var configureSimulation = AppTheme.CreateButton("Cấu hình mô phỏng", false);
        async Task ConfigureSelectedSimulationAsync()
        {
            if (grid.CurrentRow?.DataBoundItem is not StockRow stock) return;

            try
            {
                var simulations = await _stockService.GetSimulationConfigsAsync();
                var simulation = simulations.FirstOrDefault(item => item.StockId == stock.Id);
                if (simulation is null)
                {
                    MessageBox.Show(this, $"Chưa có simulation config cho {stock.Symbol}.",
                        "Không thấy cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using var dialog = new StockSimulationEditorForm(simulation);
                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                await _stockService.UpdateSimulationConfigAsync(dialog.Result);
                MessageBox.Show(this, $"Đã lưu cấu hình mô phỏng cho {stock.Symbol}.",
                    "Đã lưu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Không thể cấu hình mô phỏng",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        configureSimulation.Click += async (_, _) => await ConfigureSelectedSimulationAsync();
        edit.Click += async (_, _) => await EditSelectedStockAsync();
        grid.CellDoubleClick += async (_, eventArgs) =>
        {
            if (eventArgs.RowIndex >= 0)
            {
                await EditSelectedStockAsync();
            }
        };
        actions.Controls.Add(delete);
        actions.Controls.Add(edit);
        actions.Controls.Add(configureSimulation);
        card.Controls.Add(actions);
        page.Controls.Add(card, 0, 1);
        _ = LoadStocksAsync();
        return page;
    }

    private static StockRow ToStockRow(Stock stock)
    {
        return new StockRow
        {
            Id = stock.Id,
            Symbol = stock.Symbol,
            Company = stock.CompanyName,
            Sector = stock.Sector ?? string.Empty,
            Price = stock.CurrentPrice,
            Active = stock.IsActive
        };
    }

    private static StockUpdateDto ToUpdateDto(StockRow stock)
    {
        return new StockUpdateDto
        {
            Id = stock.Id,
            Symbol = stock.Symbol,
            CompanyName = stock.Company,
            Sector = stock.Sector,
            CurrentPrice = stock.Price,
            IsActive = stock.Active
        };
    }

    private static void ApplyStock(StockRow target, Stock source)
    {
        target.Symbol = source.Symbol;
        target.Company = source.CompanyName;
        target.Sector = source.Sector ?? string.Empty;
        target.Price = source.CurrentPrice;
        target.Active = source.IsActive;
    }

    private Control BuildSimulationPage()
    {
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Background,
            FixedPanel = FixedPanel.Panel2
        };
        split.HandleCreated += (_, _) => ApplySimulationSplitterLayout(split);
        split.SizeChanged += (_, _) => ApplySimulationSplitterLayout(split);
        var grid = AppTheme.CreateGrid();
        var simulations = new BindingList<StockSimulationConfigDto>();
        grid.DataSource = simulations;
        split.Panel1.Padding = new Padding(0, 0, 10, 0);
        split.Panel1.Controls.Add(WrapGrid(grid, "Thông số mô phỏng theo stock"));

        var editor = AppTheme.CreateCard(24);
        editor.Dock = DockStyle.Fill;
        split.Panel2.Padding = new Padding(10, 0, 0, 0);
        split.Panel2.Controls.Add(editor);
        var form = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };
        form.Controls.Add(AppTheme.CreateLabel("Cấu hình simulation", 16F, FontStyle.Bold));
        form.Controls.Add(AppTheme.CreateLabel("Chọn một stock ở bảng để chỉnh thông số.", 9F, FontStyle.Regular, AppTheme.Muted));
        var symbol = AddReadOnlyField(form, "Mã stock");
        var algorithm = AddComboField(form, "Thuật toán", ["RandomWalk", "MeanReversion", "TrendFollowing"]);
        var volatility = AddNumericField(form, "Volatility", 4, 0.02m);
        var trend = AddNumericField(form, "Trend factor", 4, 0.001m);
        var minPrice = AddNumericField(form, "Giá tối thiểu", 2, 50m, 1_000_000);
        var maxPrice = AddNumericField(form, "Giá tối đa", 2, 200m, 1_000_000);
        var speed = AddNumericField(form, "Tốc độ cập nhật (giây)", 2, 1m, 60);
        var jump = AddNumericField(form, "Xác suất jump", 4, 0.05m);
        var save = AppTheme.CreateButton("Lưu cấu hình");
        save.Width = 320;
        form.Controls.Add(save);
        editor.Controls.Add(form);

        async Task LoadSimulationsAsync()
        {
            try
            {
                var databaseSimulations = await _stockService.GetSimulationConfigsAsync();
                simulations = new BindingList<StockSimulationConfigDto>(databaseSimulations);
                grid.DataSource = simulations;
                LoadSelected();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Không thể tải cấu hình simulation",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void LoadSelected()
        {
            if (grid.CurrentRow?.DataBoundItem is not StockSimulationConfigDto simulation) return;
            symbol.Text = simulation.Symbol;
            algorithm.SelectedItem = simulation.AlgorithmType;
            volatility.Value = Clamp(volatility, simulation.Volatility);
            trend.Value = Clamp(trend, simulation.TrendFactor);
            minPrice.Value = Clamp(minPrice, simulation.MinPrice);
            maxPrice.Value = Clamp(maxPrice, simulation.MaxPrice);
            speed.Value = Clamp(speed, simulation.UpdateSpeed);
            jump.Value = Clamp(jump, simulation.JumpProbability);
        }
        grid.SelectionChanged += (_, _) => LoadSelected();
        save.Click += async (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not StockSimulationConfigDto simulation) return;
            if (!ValidateSimulationInput(minPrice.Value, maxPrice.Value, speed.Value, volatility.Value, jump.Value))
            {
                return;
            }

            try
            {
                var updated = await _stockService.UpdateSimulationConfigAsync(new StockSimulationUpdateDto
                {
                    Id = simulation.Id,
                    AlgorithmType = algorithm.Text,
                    Volatility = volatility.Value,
                    TrendFactor = trend.Value,
                    MinPrice = minPrice.Value,
                    MaxPrice = maxPrice.Value,
                    UpdateSpeed = speed.Value,
                    JumpProbability = jump.Value
                });
                ApplySimulation(simulation, updated);
                grid.Refresh();
                MessageBox.Show(this, $"Đã lưu simulation cho {simulation.Symbol}.", "Đã lưu",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Không thể lưu cấu hình simulation",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
        _ = LoadSimulationsAsync();
        return split;
    }

    private static void ApplySimulationSplitterLayout(SplitContainer split)
    {
        if (split.Width <= 0)
        {
            return;
        }

        var panel2MinSize = Math.Min(340, Math.Max(0, split.Width / 2 - split.SplitterWidth));
        var panel1MinSize = Math.Min(420, Math.Max(0, split.Width - panel2MinSize - split.SplitterWidth));
        var maxDistance = split.Width - panel2MinSize - split.SplitterWidth;
        var distance = Math.Min(520, Math.Max(panel1MinSize, maxDistance));

        split.Panel1MinSize = 0;
        split.Panel2MinSize = 0;
        split.SplitterDistance = distance;
        split.Panel1MinSize = panel1MinSize;
        split.Panel2MinSize = panel2MinSize;
    }

    private bool ValidateSimulationInput(
        decimal minPrice,
        decimal maxPrice,
        decimal speed,
        decimal volatility,
        decimal jumpProbability)
    {
        if (minPrice >= maxPrice)
        {
            MessageBox.Show(this, "Giá tối thiểu phải nhỏ hơn giá tối đa.",
                "Cấu hình simulation không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (speed <= 0)
        {
            MessageBox.Show(this, "Tốc độ cập nhật phải lớn hơn 0.",
                "Cấu hình simulation không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (volatility < 0 || volatility > 1 || jumpProbability < 0 || jumpProbability > 1)
        {
            MessageBox.Show(this, "Volatility và xác suất jump phải nằm trong khoảng 0 đến 1.",
                "Cấu hình simulation không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private static void ApplySimulation(StockSimulationConfigDto target, StockSimulationConfigDto source)
    {
        target.AlgorithmType = source.AlgorithmType;
        target.Volatility = source.Volatility;
        target.TrendFactor = source.TrendFactor;
        target.MinPrice = source.MinPrice;
        target.MaxPrice = source.MaxPrice;
        target.UpdateSpeed = source.UpdateSpeed;
        target.JumpProbability = source.JumpProbability;
        target.UpdatedAt = source.UpdatedAt;
    }
}
