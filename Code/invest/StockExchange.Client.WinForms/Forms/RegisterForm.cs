using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Services;

namespace StockExchange.Client.WinForms.Forms;

public class RegisterForm : Form
{
    private readonly AuthClientService _authService;

    public RegisterForm(AuthClientService authService)
    {
        _authService = authService;
        Text = "Invest App - Đăng ký";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(620, 650);
        MinimumSize = new Size(560, 600);
        BackColor = AppTheme.Background;
        Font = AppTheme.BodyFont;
        Padding = new Padding(AppTheme.SpaceXl);
        AutoScaleMode = AutoScaleMode.Font;

        var card = AppTheme.CreateCard(AppTheme.SpaceXl);
        card.Dock = DockStyle.Fill;
        Controls.Add(card);

        var layout = AppTheme.CreatePage(3);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        card.Controls.Add(layout);

        var header = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, AppTheme.SpaceLg)
        };
        header.Controls.Add(AppTheme.CreateLabel("Tạo tài khoản", 20F, FontStyle.Bold));
        header.Controls.Add(AppTheme.CreateLabel(
            "Nhập thông tin để tạo tài khoản Invest App.",
            9.5F,
            FontStyle.Regular,
            AppTheme.Muted));
        layout.Controls.Add(header, 0, 0);

        var fields = AppTheme.CreateFormGrid(170);
        var username = AppTheme.CreateTextBox("Ví dụ: tencuaban");
        var email = AppTheme.CreateTextBox("mail-cua-ban@vidu.com");
        var password = AppTheme.CreateTextBox("Tối thiểu 6 ký tự");
        password.UseSystemPasswordChar = true;
        var confirm = AppTheme.CreateTextBox("Nhập lại mật khẩu");
        confirm.UseSystemPasswordChar = true;
        AppTheme.AddField(fields, "Tên đăng nhập", username);
        AppTheme.AddField(fields, "Email", email);
        AppTheme.AddField(fields, "Mật khẩu", password);
        AppTheme.AddField(fields, "Xác nhận mật khẩu", confirm);

        var terms = AppTheme.CreateCheckBox("Tôi đồng ý với điều khoản sử dụng");
        AppTheme.AddField(fields, string.Empty, terms);
        layout.Controls.Add(fields, 0, 1);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Margin = new Padding(0, AppTheme.SpaceMd, 0, 0)
        };
        var submit = AppTheme.CreateButton("Tạo tài khoản");
        submit.Width = 160;
        submit.Click += async (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(username.Text) || string.IsNullOrWhiteSpace(email.Text)
                || string.IsNullOrWhiteSpace(password.Text) || password.Text != confirm.Text || !terms.Checked)
            {
                MessageBox.Show(this, "Vui lòng kiểm tra lại thông tin đăng ký.", "Thông tin chưa hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            submit.Enabled = false;
            try
            {
                var response = await _authService.RegisterAsync(
                    username.Text.Trim(),
                    email.Text.Trim(),
                    password.Text);
                if (!response.Success)
                {
                    MessageBox.Show(this, response.Message, "Đăng ký thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(this, "Đăng ký tài khoản thành công. Bạn có thể đăng nhập ngay.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Lỗi kết nối",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                submit.Enabled = true;
            }
        };
        var back = AppTheme.CreateButton("Quay lại", false);
        back.Width = 120;
        back.Click += (_, _) => Close();
        actions.Controls.Add(submit);
        actions.Controls.Add(back);
        layout.Controls.Add(actions, 0, 2);

        AcceptButton = submit;
        CancelButton = back;
        Shown += (_, _) => AppTheme.ApplyCommonStyles(this);
    }
}
