using System.ComponentModel;
using StockExchange.Client.WinForms.Controls;
using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Mock;

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
        summary.Controls.Add(BuildStatCard("Thanh khoản", "18,420 tỷ", "Khối lượng 642M", AppTheme.Primary));
        summary.Controls.Add(BuildStatCard("Độ rộng", "238 tăng", "92 giảm • 61 tham chiếu", AppTheme.Warning));
        page.Controls.Add(summary, 0, 0);

        var toolbar = BuildToolbar("Tìm kiếm cổ phiếu...", "Bộ lọc", out var search, out var filter);
        filter.Click += (_, _) => AppTheme.ShowTemplateNotice(this, "Lọc theo ngành và trạng thái");
        page.Controls.Add(toolbar, 0, 1);

        var table = new StockTableControl();
        void Bind(string keyword = "")
        {
            table.SetData(new BindingList<StockRow>(MockData.Stocks
                .Where(stock => stock.Active && (string.IsNullOrWhiteSpace(keyword)
                    || stock.Symbol.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || stock.Company.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToList()));
        }
        Bind();
        search.TextChanged += (_, _) => Bind(search.Text);
        table.StockSelected += (_, stock) =>
        {
            _selectedStock = stock;
            Navigate("Chi tiết stock");
        };
        page.Controls.Add(WrapControl(table, "Bảng giá • Double click để xem chi tiết"), 0, 2);
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
        list.SetStocks(MockData.Watchlist);
        page.Controls.Add(WrapControl(list, "Danh sách theo dõi"), 0, 0);

        var detailHost = AppTheme.CreateCard();
        detailHost.Dock = DockStyle.Fill;
        detailHost.Margin = new Padding(AppTheme.SpaceMd, AppTheme.SpaceSm, AppTheme.SpaceSm, AppTheme.SpaceSm);
        page.Controls.Add(detailHost, 1, 0);

        void ShowMiniDetail(StockRow stock)
        {
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
            header.Controls.Add(AppTheme.CreateLabel(
                $"{stock.ChangePercent:+0.00;-0.00;0.00}%",
                12F,
                FontStyle.Bold,
                stock.ChangePercent >= 0 ? AppTheme.Success : AppTheme.Danger), 1, 0);

            var summary = new StockSummaryControl();
            summary.SetStock(stock);
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
        if (MockData.Watchlist.FirstOrDefault() is { } initialStock)
        {
            list.SelectStock(initialStock);
        }
        else
        {
            detailHost.Controls.Add(new Label
            {
                Text = "Chọn cổ phiếu từ thị trường để thêm vào watchlist.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = AppTheme.BodyFont,
                ForeColor = AppTheme.Muted
            });
        }

        return page;
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
        summaryFlow.Controls.Add(new Label
        {
            Text = _username[..1].ToUpperInvariant(),
            AutoSize = false,
            Size = new Size(92, 92),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = AppTheme.PrimarySoft,
            ForeColor = AppTheme.Primary,
            Font = AppTheme.CreateFont(36F, FontStyle.Bold),
            Margin = new Padding(48, 15, 0, 18)
        });
        summaryFlow.Controls.Add(AppTheme.CreateLabel(_username, 16F, FontStyle.Bold));
        summaryFlow.Controls.Add(AppTheme.CreateLabel(_isAdmin ? "Administrator" : "Member", 10F, FontStyle.Bold, AppTheme.Primary));
        summaryFlow.Controls.Add(AppTheme.CreateLabel("Thành viên từ 2026", 9F, FontStyle.Regular, AppTheme.Muted));
        summaryFlow.Controls.Add(new Panel { Width = 1, Height = 20 });
        summaryFlow.Controls.Add(AppTheme.CreateLabel($"Watchlist: {MockData.Watchlist.Count} mã", 10F, FontStyle.Regular));
        summaryFlow.Controls.Add(AppTheme.CreateLabel("Trạng thái: Active", 10F, FontStyle.Regular, AppTheme.Success));
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
        var email = AddTextField(form, "Email", $"{_username}@example.com");
        var displayName = AddTextField(form, "Tên hiển thị", "Ngô Quốc Kha");
        var phone = AddTextField(form, "Số điện thoại", "0900 000 000");
        var save = AppTheme.CreateButton("Lưu thay đổi");
        save.Width = 360;
        save.Click += (_, _) => AppTheme.ShowTemplateNotice(this, "Cập nhật hồ sơ");
        form.Controls.Add(save);
        var password = AppTheme.CreateButton("Đổi mật khẩu", false);
        password.Width = 360;
        password.Click += (_, _) => AppTheme.ShowTemplateNotice(this, "Đổi mật khẩu");
        form.Controls.Add(password);
        editor.Controls.Add(form);
        page.Controls.Add(editor, 1, 0);
        return page;
    }

}
