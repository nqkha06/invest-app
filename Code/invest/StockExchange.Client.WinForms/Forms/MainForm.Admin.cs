using System.ComponentModel;
using StockExchange.Client.WinForms.Controls;
using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Mock;

namespace StockExchange.Client.WinForms.Forms;

public partial class MainForm : Form
{
    private Control BuildAdminDashboard()
    {
        var page = AppTheme.CreatePage(3);
        page.AutoScroll = true;
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 170));
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 360));
        page.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var stats = new EqualColumnPanel
        {
            Dock = DockStyle.Fill,
            Columns = 4,
            Gap = 12,
            Padding = new Padding(0, 0, 0, 12)
        };
        stats.Controls.Add(BuildStatCard("Người dùng", MockData.Users.Count.ToString(), "+12% tháng này", AppTheme.Primary));
        stats.Controls.Add(BuildStatCard("Stock đang hoạt động", MockData.Stocks.Count(stock => stock.Active).ToString(), "5 mã được seed", AppTheme.Success));
        stats.Controls.Add(BuildStatCard("Simulation", MockData.Simulations.Count.ToString(), "RandomWalk", AppTheme.Warning));
        stats.Controls.Add(BuildStatCard("Kết nối", "128", "Mock concurrent clients", Color.FromArgb(124, 58, 237)));
        page.Controls.Add(stats, 0, 0);

        var middle = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        };
        middle.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        middle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64));
        middle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36));
        var activitySummary = new EqualColumnPanel
        {
            Dock = DockStyle.Fill,
            Columns = 2,
            Gap = AppTheme.SpaceMd
        };
        activitySummary.Controls.Add(BuildStatCard("Kết nối hiện tại", "128", "Client đang hoạt động", AppTheme.Primary));
        activitySummary.Controls.Add(BuildStatCard("Cập nhật giá", "2.4K", "Trong 7 ngày", AppTheme.Success));
        middle.Controls.Add(WrapControl(activitySummary, "Tóm tắt hoạt động hệ thống"), 0, 0);

        var health = AppTheme.CreateCard();
        health.Dock = DockStyle.Fill;
        var healthList = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };
        healthList.Controls.Add(AppTheme.CreateLabel("Tình trạng service", 14F, FontStyle.Bold));
        healthList.Controls.Add(BuildHealthRow("AuthService", "Sẵn sàng"));
        healthList.Controls.Add(BuildHealthRow("StockService", "Sẵn sàng"));
        healthList.Controls.Add(BuildHealthRow("WatchlistService", "Sẵn sàng"));
        healthList.Controls.Add(BuildHealthRow("PriceHistoryService", "Chờ tích hợp"));
        healthList.Controls.Add(BuildHealthRow("TCP Server", "Chờ tích hợp"));
        health.Controls.Add(healthList);
        middle.Controls.Add(health, 1, 0);
        page.Controls.Add(middle, 0, 1);

        var grid = AppTheme.CreateGrid();
        grid.DataSource = new BindingList<UserRow>(MockData.Users.OrderByDescending(user => user.CreatedAt).Take(4).ToList());
        page.Controls.Add(WrapGrid(grid, "Người dùng mới gần đây"), 0, 2);
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
        void Bind(string keyword = "")
        {
            grid.DataSource = new BindingList<UserRow>(MockData.Users
                .Where(user => string.IsNullOrWhiteSpace(keyword)
                    || user.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || user.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList());
        }
        Bind();
        search.TextChanged += (_, _) => Bind(search.Text);
        add.Click += (_, _) =>
        {
            using var dialog = new UserEditorForm();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                MockData.Users.Add(dialog.Result);
                Bind(search.Text);
            }
        };
        grid.CellDoubleClick += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is UserRow user)
            {
                AppTheme.ShowTemplateNotice(this, $"Chỉnh sửa người dùng {user.Username}");
            }
        };
        page.Controls.Add(WrapGrid(grid, "Danh sách người dùng • Double click để chỉnh sửa"), 0, 1);
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
        void Bind(string keyword = "")
        {
            grid.DataSource = new BindingList<StockRow>(MockData.Stocks
                .Where(stock => string.IsNullOrWhiteSpace(keyword)
                    || stock.Symbol.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || stock.Company.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || stock.Sector.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList());
        }
        Bind();
        search.TextChanged += (_, _) => Bind(search.Text);
        add.Click += (_, _) =>
        {
            using var dialog = new StockEditorForm();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                MockData.Stocks.Add(dialog.Result);
                Bind(search.Text);
            }
        };

        var card = WrapGrid(grid, "Quản lý stock • Dữ liệu local trong phiên");
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 68,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 6, 8, 6)
        };
        var delete = AppTheme.CreateButton("Xóa", false);
        delete.ForeColor = AppTheme.Danger;
        delete.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not StockRow stock) return;
            MockData.Stocks.Remove(stock);
            Bind(search.Text);
        };
        var edit = AppTheme.CreateButton("Chỉnh sửa");
        edit.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is StockRow stock)
            {
                AppTheme.ShowTemplateNotice(this, $"Chỉnh sửa stock {stock.Symbol}");
            }
        };
        actions.Controls.Add(delete);
        actions.Controls.Add(edit);
        card.Controls.Add(actions);
        page.Controls.Add(card, 0, 1);
        return page;
    }

    private Control BuildSimulationPage()
    {
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 520,
            BackColor = AppTheme.Background,
            FixedPanel = FixedPanel.Panel2,
            Panel1MinSize = 420,
            Panel2MinSize = 340
        };
        var grid = AppTheme.CreateGrid();
        grid.DataSource = MockData.Simulations;
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

        void LoadSelected()
        {
            if (grid.CurrentRow?.DataBoundItem is not SimulationRow simulation) return;
            symbol.Text = simulation.Symbol;
            algorithm.SelectedItem = simulation.Algorithm;
            volatility.Value = Clamp(volatility, simulation.Volatility);
            trend.Value = Clamp(trend, simulation.TrendFactor);
            minPrice.Value = Clamp(minPrice, simulation.MinPrice);
            maxPrice.Value = Clamp(maxPrice, simulation.MaxPrice);
            speed.Value = Clamp(speed, simulation.UpdateSpeed);
            jump.Value = Clamp(jump, simulation.JumpProbability);
        }
        grid.SelectionChanged += (_, _) => LoadSelected();
        save.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not SimulationRow simulation) return;
            simulation.Algorithm = algorithm.Text;
            simulation.Volatility = volatility.Value;
            simulation.TrendFactor = trend.Value;
            simulation.MinPrice = minPrice.Value;
            simulation.MaxPrice = maxPrice.Value;
            simulation.UpdateSpeed = speed.Value;
            simulation.JumpProbability = jump.Value;
            grid.Refresh();
            AppTheme.ShowTemplateNotice(this, $"Lưu simulation cho {simulation.Symbol}");
        };
        LoadSelected();
        return split;
    }

}
