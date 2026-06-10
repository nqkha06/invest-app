using System.ComponentModel;
using StockExchange.Client.WinForms.Controls;
using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Mock;

namespace StockExchange.Client.WinForms.Forms;

public partial class MainForm : Form
{
    private readonly string _username;
    private readonly bool _isAdmin;
    private readonly Panel _content = new() { Dock = DockStyle.Fill, BackColor = AppTheme.Background, Padding = new Padding(AppTheme.SpaceXl) };
    private readonly Label _pageTitle = AppTheme.CreateLabel("", 18F, FontStyle.Bold);
    private readonly Dictionary<string, Button> _navButtons = [];
    private StockRow _selectedStock = MockData.Stocks[0];

    public MainForm(string username, bool isAdmin)
    {
        _username = username;
        _isAdmin = isAdmin;

        Text = $"Invest App - {(isAdmin ? "Admin" : "Member")}";
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(900, 620);
        BackColor = AppTheme.Background;
        Font = AppTheme.BodyFont;
        AutoScaleMode = AutoScaleMode.Font;

        var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240));
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        Controls.Add(shell);

        shell.Controls.Add(BuildSidebar(), 0, 0);
        shell.Controls.Add(BuildWorkspace(), 1, 0);
        Navigate(_isAdmin ? "Tổng quan" : "Thị trường");
        Shown += (_, _) => AppTheme.ApplyCommonStyles(this);
    }

    private Control BuildSidebar()
    {
        var sidebar = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Sidebar, Padding = new Padding(16, 20, 16, 16) };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 108));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 128));
        sidebar.Controls.Add(layout);

        var brand = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        brand.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        brand.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        brand.Controls.Add(new Label
        {
            Text = "INVEST",
            AutoSize = true,
            Font = AppTheme.CreateFont(26F, FontStyle.Bold),
            ForeColor = Color.White,
            Margin = new Padding(8, 4, 0, 2)
        }, 0, 0);
        brand.Controls.Add(new Label
        {
            Text = _isAdmin ? "ADMIN CONSOLE" : "MARKET TERMINAL",
            AutoSize = true,
            Font = AppTheme.CreateFont(12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(96, 165, 250),
            Margin = new Padding(10, 0, 0, 0)
        }, 0, 1);
        layout.Controls.Add(brand, 0, 0);

        var menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };
        var items = _isAdmin
            ? new[] { "Tổng quan", "Người dùng", "Cổ phiếu", "Mô phỏng" }
            : new[] { "Thị trường", "Chi tiết stock", "Watchlist", "Hồ sơ" };
        foreach (var item in items)
        {
            menu.Controls.Add(CreateNavButton(item));
        }
        layout.Controls.Add(menu, 0, 1);

        var account = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            Padding = new Padding(4, 8, 0, 0)
        };
        account.RowStyles.Add(new RowStyle(SizeType.Percent, 34F));
        account.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
        account.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
        account.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52));
        account.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        account.Controls.Add(new Label
        {
            Text = _username[..1].ToUpperInvariant(),
            AutoSize = false,
            Size = new Size(42, 42),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = AppTheme.Primary,
            ForeColor = Color.White,
            Font = AppTheme.CreateFont(17F, FontStyle.Bold),
            Anchor = AnchorStyles.Top | AnchorStyles.Left
        }, 0, 0);
        account.SetRowSpan(account.Controls[^1], 3);
        account.Controls.Add(new Label
        {
            Text = _username,
            AutoSize = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            ForeColor = Color.White,
            Font = AppTheme.CreateFont(15F, FontStyle.Bold)
        }, 1, 0);
        account.Controls.Add(new Label
        {
            Text = _isAdmin ? "Administrator" : "Member",
            AutoSize = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            ForeColor = Color.FromArgb(148, 163, 184),
            Font = AppTheme.SmallFont
        }, 1, 1);
        var logout = new LinkLabel
        {
            Text = "Đăng xuất",
            AutoSize = true,
            LinkColor = Color.FromArgb(147, 197, 253),
            Font = AppTheme.SmallFont,
            Anchor = AnchorStyles.Left
        };
        logout.Click += (_, _) => Close();
        account.Controls.Add(logout, 1, 2);
        layout.Controls.Add(account, 0, 2);
        return sidebar;
    }

    private Button CreateNavButton(string text)
    {
        var button = new Button
        {
            Text = text,
            TextAlign = ContentAlignment.MiddleLeft,
            Width = 200,
            Height = 48,
            FlatStyle = FlatStyle.Flat,
            BackColor = AppTheme.Sidebar,
            ForeColor = Color.FromArgb(203, 213, 225),
            Font = AppTheme.CreateFont(15F, FontStyle.Bold),
            Padding = new Padding(14, 0, 6, 0),
            Margin = new Padding(0, 4, 0, 4),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => Navigate(text);
        _navButtons[text] = button;
        return button;
    }

    private Control BuildWorkspace()
    {
        var workspace = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        workspace.RowStyles.Add(new RowStyle(SizeType.Absolute, 88));
        workspace.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(AppTheme.SpaceXl, AppTheme.SpaceLg, AppTheme.SpaceXl, AppTheme.SpaceSm),
            ColumnCount = 2,
            RowCount = 1
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _pageTitle.Anchor = AnchorStyles.Left;
        _pageTitle.Margin = Padding.Empty;
        header.Controls.Add(_pageTitle, 0, 0);
        header.Controls.Add(new Label
        {
            Text = "Dữ liệu minh họa • Chưa kết nối server",
            AutoSize = true,
            Anchor = AnchorStyles.Right,
            ForeColor = AppTheme.Warning,
            Font = AppTheme.CreateFont(13F, FontStyle.Bold),
            Margin = Padding.Empty
        }, 1, 0);

        workspace.Controls.Add(header, 0, 0);
        workspace.Controls.Add(_content, 0, 1);
        return workspace;
    }

    private void Navigate(string page)
    {
        _pageTitle.Text = page;
        foreach (var pair in _navButtons)
        {
            var active = pair.Key == page;
            pair.Value.BackColor = active ? Color.FromArgb(30, 64, 175) : AppTheme.Sidebar;
            pair.Value.ForeColor = active ? Color.White : Color.FromArgb(203, 213, 225);
        }

        _content.Controls.Clear();
        Control view = page switch
        {
            "Tổng quan" => BuildAdminDashboard(),
            "Người dùng" => BuildUsersPage(),
            "Cổ phiếu" => BuildAdminStocksPage(),
            "Mô phỏng" => BuildSimulationPage(),
            "Thị trường" => BuildMarketPage(),
            "Chi tiết stock" => BuildStockDetailPage(_selectedStock),
            "Watchlist" => BuildWatchlistPage(),
            "Hồ sơ" => BuildProfilePage(),
            _ => new Panel()
        };
        view.Dock = DockStyle.Fill;
        _content.Controls.Add(view);
    }

}
