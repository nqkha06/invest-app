using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Services;

namespace StockExchange.Client.WinForms.Forms;

public class LoginForm : Form
{
    private readonly ClientConnectionService _connection = new();
    private readonly AuthClientService _authService;
    private readonly TextBox _username = AppTheme.CreateTextBox("Tên đăng nhập hoặc email");
    private readonly TextBox _password = AppTheme.CreateTextBox("Mật khẩu");

    public LoginForm()
    {
        _authService = new AuthClientService(_connection);
        Text = "Invest App - Đăng nhập";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1050, 680);
        Size = new Size(1180, 760);
        BackColor = AppTheme.Background;
        Font = AppTheme.BodyFont;
        AutoScaleMode = AutoScaleMode.Font;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48));
        Controls.Add(layout);

        layout.Controls.Add(BuildHero(), 0, 0);
        layout.Controls.Add(BuildLoginPanel(), 1, 0);
    }

    private Control BuildHero()
    {
        var hero = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Sidebar,
            Padding = new Padding(48)
        };

        var content = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(0, 48, 0, 0),
            AutoScroll = true
        };
        content.Controls.Add(AppTheme.CreateLabel("INVEST APP", 12F, FontStyle.Bold, Color.FromArgb(96, 165, 250)));
        content.Controls.Add(new Label
        {
            Text = "Theo dõi thị trường.\nRa quyết định rõ ràng.",
            AutoSize = true,
            MaximumSize = new Size(500, 0),
            Font = AppTheme.CreateFont(36F, FontStyle.Bold),
            ForeColor = Color.White,
            Margin = new Padding(0, 18, 0, 24)
        });
        content.Controls.Add(new Label
        {
            Text = "Template giao diện cho hệ thống mô phỏng sàn chứng khoán: bảng giá, watchlist, biểu đồ, quản trị người dùng và cấu hình simulation.",
            AutoSize = true,
            MaximumSize = new Size(470, 0),
            Font = AppTheme.CreateFont(16F),
            ForeColor = Color.FromArgb(203, 213, 225),
            Margin = new Padding(0, 0, 0, 32)
        });
        content.Controls.Add(BuildFeature("01", "Dữ liệu thị trường", "Theo dõi giá và biến động cổ phiếu."));
        content.Controls.Add(BuildFeature("02", "Danh mục quan tâm", "Quản lý watchlist cá nhân."));
        content.Controls.Add(BuildFeature("03", "Mô phỏng giá", "Admin cấu hình thuật toán simulation."));
        hero.Controls.Add(content);
        return hero;
    }

    private static Control BuildFeature(string number, string title, string description)
    {
        var row = new TableLayoutPanel
        {
            Width = 430,
            Height = 84,
            ColumnCount = 2,
            RowCount = 2,
            Margin = new Padding(0, 6, 0, 6)
        };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        row.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        row.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        var badge = new Label
        {
            Text = number,
            AutoSize = false,
            Size = new Size(44, 44),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.FromArgb(30, 41, 59),
            ForeColor = Color.FromArgb(96, 165, 250),
            Font = AppTheme.CreateFont(15F, FontStyle.Bold),
            Anchor = AnchorStyles.Left
        };
        row.Controls.Add(badge, 0, 0);
        row.SetRowSpan(badge, 2);
        row.Controls.Add(new Label
        {
            Text = title,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
            ForeColor = Color.White,
            Font = AppTheme.CreateFont(16F, FontStyle.Bold),
            AutoEllipsis = true
        }, 1, 0);
        row.Controls.Add(new Label
        {
            Text = description,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft,
            ForeColor = Color.FromArgb(148, 163, 184),
            Font = AppTheme.SmallFont,
            AutoEllipsis = true
        }, 1, 1);
        return row;
    }

    private Control BuildLoginPanel()
    {
        var host = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Background,
            ColumnCount = 3,
            RowCount = 3
        };
        host.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        host.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 430));
        host.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        host.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        host.RowStyles.Add(new RowStyle(SizeType.Absolute, 580));
        host.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        var card = AppTheme.CreateCard(36);
        card.Dock = DockStyle.Fill;
        host.Controls.Add(card, 1, 1);

        var form = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };
        form.Controls.Add(AppTheme.CreateLabel("Chào mừng trở lại", 22F, FontStyle.Bold));
        form.Controls.Add(AppTheme.CreateLabel("Đăng nhập để tiếp tục vào Invest App.", 10F, FontStyle.Regular, AppTheme.Muted));
        form.Controls.Add(Spacer(18));
        form.Controls.Add(AppTheme.CreateLabel("Tên đăng nhập / Email", 9.5F, FontStyle.Bold));
        _username.Width = 330;
        _username.Text = "demo_user";
        form.Controls.Add(_username);
        form.Controls.Add(AppTheme.CreateLabel("Mật khẩu", 9.5F, FontStyle.Bold));
        _password.Width = 330;
        _password.UseSystemPasswordChar = true;
        _password.Text = "123456";
        form.Controls.Add(_password);

        var options = new TableLayoutPanel { Width = 330, Height = 44, ColumnCount = 2 };
        options.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
        options.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        options.Controls.Add(new CheckBox { Text = "Ghi nhớ đăng nhập", AutoSize = true, ForeColor = AppTheme.Muted, Anchor = AnchorStyles.Left }, 0, 0);
        var forgot = new LinkLabel
        {
            Text = "Quên mật khẩu?",
            AutoSize = true,
            LinkColor = AppTheme.Primary,
            Anchor = AnchorStyles.Right
        };
        forgot.Click += (_, _) => AppTheme.ShowTemplateNotice(this, "Khôi phục mật khẩu");
        options.Controls.Add(forgot, 1, 0);
        form.Controls.Add(options);

        var login = AppTheme.CreateButton("Đăng nhập");
        login.Width = 330;
        login.Click += async (_, _) => await OpenApplicationAsync(login);
        form.Controls.Add(login);
        form.Controls.Add(AppTheme.CreateLabel("Dùng username “admin” để xem giao diện Admin.", 9F, FontStyle.Italic, AppTheme.Muted));

        var register = new LinkLabel
        {
            Text = "Chưa có tài khoản? Đăng ký ngay",
            AutoSize = true,
            LinkColor = AppTheme.Primary,
            Font = AppTheme.CreateFont(14F, FontStyle.Bold),
            Margin = new Padding(0, 18, 0, 0)
        };
        register.Click += (_, _) => new RegisterForm(_authService).ShowDialog(this);
        form.Controls.Add(register);
        card.Controls.Add(form);
        AcceptButton = login;
        Shown += (_, _) => AppTheme.ApplyCommonStyles(this);
        return host;
    }

    private async Task OpenApplicationAsync(Button loginButton)
    {
        if (string.IsNullOrWhiteSpace(_username.Text) || string.IsNullOrWhiteSpace(_password.Text))
        {
            MessageBox.Show(this, "Vui lòng nhập đầy đủ thông tin.", "Thiếu thông tin",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        loginButton.Enabled = false;
        try
        {
            var response = await _authService.LoginAsync(_username.Text.Trim(), _password.Text);
            if (!response.Success)
            {
                MessageBox.Show(this, response.Message, "Đăng nhập thất bại",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Hide();
            using var main = new MainForm(response, _authService);
            main.ShowDialog();
            Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Lỗi kết nối",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            loginButton.Enabled = true;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        base.Dispose(disposing);
    }

    private static Control Spacer(int height) => new Panel { Width = 1, Height = height };
}
