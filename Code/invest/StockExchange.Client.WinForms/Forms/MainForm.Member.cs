using StockExchange.Client.WinForms.Controls;
using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Mock;
using StockExchange.Shared.DTOs;

namespace StockExchange.Client.WinForms.Forms;

public partial class MainForm : Form
{
    private Control BuildMarketPage()
    {
        var page = AppTheme.CreatePage(3);
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        page.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var summary = new EqualColumnPanel
        {
            Dock = DockStyle.Fill,
            Columns = 3,
            Gap = 12,
            Padding = new Padding(0, 0, 0, 10)
        };
        summary.Controls.Add(BuildStatCard("VN-INDEX", "1,284.21", "+8.42 (+0.66%)", AppTheme.Success));
        summary.Controls.Add(BuildStatCard("Thanh khoản", "18,420 tỷ", "Volume 642M", AppTheme.Primary));
        summary.Controls.Add(BuildStatCard("Độ rộng thị trường", "238 tăng", "92 giảm - 61 đứng giá", AppTheme.Warning));
        page.Controls.Add(summary, 0, 0);

        var toolbar = BuildToolbar("Tìm kiếm cổ phiếu...", "Làm mới", out var search, out var refresh);
        refresh.Click += async (_, _) => await RefreshStocksAsync();
        page.Controls.Add(toolbar, 0, 1);

        var table = new StockTableControl();
        _marketTable = table;
        RebindMarket();
        _ = RefreshStocksAsync();

        search.TextChanged += (_, _) =>
        {
            _marketSearch = search.Text;
            RebindMarket();
        };
        table.StockSelected += (_, stock) =>
        {
            _selectedStock = stock;
            Navigate("Chi tiết stock");
        };

        page.Controls.Add(WrapControl(table, "Bảng giá - Nhấp đúp để xem chi tiết"), 0, 2);
        return page;
    }

    private Control BuildWatchlistPage()
    {
        var page = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Background,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        };
        page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36F));
        page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64F));
        page.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var list = new WatchlistControl { Dock = DockStyle.Fill };
        _watchlistControl = list;
        var watchlistStocks = GetWatchlistStocks();
        list.SetStocks(watchlistStocks);
        page.Controls.Add(WrapControl(list, "Danh sách theo dõi"), 0, 0);

        var detailHost = AppTheme.CreateCard();
        detailHost.Dock = DockStyle.Fill;
        detailHost.Margin = new Padding(AppTheme.SpaceMd, AppTheme.SpaceSm, AppTheme.SpaceSm, AppTheme.SpaceSm);
        page.Controls.Add(detailHost, 1, 0);

        void ShowMiniDetail(StockRow stock)
        {
            _watchlistSelectedStock = stock;
            detailHost.Controls.Clear();
            var layout = AppTheme.CreatePage(3);
            layout.BackColor = AppTheme.Surface;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var header = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, AppTheme.SpaceMd)
            };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            header.Controls.Add(AppTheme.CreateLabel($"{stock.Symbol} - {stock.Company}", 18F, FontStyle.Bold), 0, 0);

            var summary = new StockSummaryControl();
            summary.SetStock(stock);
            _watchlistSummary = summary;

            var open = AppTheme.CreateButton("Xem chi tiết");
            open.Anchor = AnchorStyles.Right;
            open.Margin = new Padding(0, AppTheme.SpaceMd, 0, 0);
            open.Click += (_, _) =>
            {
                _selectedStock = stock;
                Navigate("Chi tiết stock");
            };
            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };
            actions.Controls.Add(open);

            layout.Controls.Add(header, 0, 0);
            layout.Controls.Add(summary, 0, 1);
            layout.Controls.Add(actions, 0, 2);
            detailHost.Controls.Add(layout);
        }

        list.StockSelected += (_, stock) => ShowMiniDetail(stock);
        if (watchlistStocks.FirstOrDefault() is { } initialStock)
        {
            list.SelectStock(initialStock);
        }
        else
        {
            detailHost.Controls.Add(new Label
            {
                Text = "Chọn cổ phiếu từ thị trường để xem watchlist.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = AppTheme.BodyFont,
                ForeColor = AppTheme.Muted
            });
        }

        return page;
    }

    private List<StockRow> GetWatchlistStocks()
    {
        var symbols = MockData.Watchlist.Select(stock => stock.Symbol).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var stocks = _stocks.Where(stock => symbols.Contains(stock.Symbol)).ToList();
        return stocks.Count == 0 ? MockData.Watchlist.ToList() : stocks;
    }

    private Control BuildProfilePage()
    {
        var page = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Background
        };
        page.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 330));
        page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var summary = AppTheme.CreateCard();
        summary.Dock = DockStyle.Fill;
        var summaryFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(AppTheme.SpaceLg)
        };
        var avatar = new Label
        {
            Text = _username[..1].ToUpperInvariant(),
            AutoSize = false,
            Size = new Size(92, 92),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = AppTheme.PrimarySoft,
            ForeColor = AppTheme.Primary,
            Font = AppTheme.CreateFont(36F, FontStyle.Bold),
            Margin = new Padding(48, 15, 0, 18)
        };
        var summaryName = AppTheme.CreateLabel(_username, 16F, FontStyle.Bold);
        var memberSince = AppTheme.CreateLabel("Đang tải thông tin...", 9F, FontStyle.Regular, AppTheme.Muted);
        var status = AppTheme.CreateLabel("Trạng thái: Hoạt động", 10F, FontStyle.Regular, AppTheme.Success);
        summaryFlow.Controls.Add(avatar);
        summaryFlow.Controls.Add(summaryName);
        summaryFlow.Controls.Add(AppTheme.CreateLabel(_isAdmin ? "Quản trị viên" : "Thành viên", 10F, FontStyle.Bold, AppTheme.Primary));
        summaryFlow.Controls.Add(memberSince);
        summaryFlow.Controls.Add(new Panel { Width = 1, Height = 20 });
        summaryFlow.Controls.Add(AppTheme.CreateLabel($"Watchlist: {GetWatchlistStocks().Count} mã", 10F, FontStyle.Regular));
        summaryFlow.Controls.Add(status);
        summary.Controls.Add(summaryFlow);
        page.Controls.Add(summary, 0, 0);

        var editor = AppTheme.CreateCard(30);
        editor.Dock = DockStyle.Fill;
        var form = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };
        form.Controls.Add(AppTheme.CreateLabel("Thông tin cá nhân", 17F, FontStyle.Bold));
        form.Controls.Add(AppTheme.CreateLabel("Cập nhật thông tin cơ bản của tài khoản.", 9.5F, FontStyle.Regular, AppTheme.Muted));
        form.Controls.Add(new Panel { Width = 1, Height = 14 });
        var username = AddTextField(form, "Tên đăng nhập", _username);
        var email = AddTextField(form, "Email", _profile.Email);
        var save = AppTheme.CreateButton("Lưu thay đổi");
        save.Width = 360;
        save.Click += async (_, _) =>
        {
            save.Enabled = false;
            try
            {
                _profile = await _authService.UpdateProfileAsync(username.Text.Trim(), email.Text.Trim());
                ApplyProfile(_profile, username, email, avatar, summaryName, memberSince, status);
                MessageBox.Show(this, "Cập nhật hồ sơ thành công.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Không thể cập nhật hồ sơ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                save.Enabled = true;
            }
        };
        form.Controls.Add(save);
        editor.Controls.Add(form);
        page.Controls.Add(editor, 1, 0);
        _ = LoadProfileAsync(username, email, avatar, summaryName, memberSince, status);
        return page;
    }

    private async Task LoadProfileAsync(
        TextBox username,
        TextBox email,
        Label avatar,
        Label summaryName,
        Label memberSince,
        Label status)
    {
        try
        {
            _profile = await _authService.GetProfileAsync();
            if (!IsDisposed && !username.IsDisposed)
            {
                ApplyProfile(_profile, username, email, avatar, summaryName, memberSince, status);
            }
        }
        catch (Exception ex)
        {
            if (!IsDisposed && !username.IsDisposed)
            {
                MessageBox.Show(this, ex.Message, "Không thể tải hồ sơ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    private void ApplyProfile(
        UserProfileDto profile,
        TextBox username,
        TextBox email,
        Label avatar,
        Label summaryName,
        Label memberSince,
        Label status)
    {
        _username = profile.Username;
        username.Text = profile.Username;
        email.Text = profile.Email;
        avatar.Text = !string.IsNullOrEmpty(profile.Username) ? profile.Username[..1].ToUpperInvariant() : "?";
        summaryName.Text = profile.Username;
        memberSince.Text = $"Thành viên từ {profile.CreatedAt:yyyy}";
        status.Text = profile.IsActive ? "Trạng thái: Hoạt động" : "Trạng thái: Ngừng hoạt động";
        status.ForeColor = profile.IsActive ? AppTheme.Success : AppTheme.Danger;
        _accountName.Text = profile.Username;
        _accountAvatar.Text = avatar.Text;
    }
}
