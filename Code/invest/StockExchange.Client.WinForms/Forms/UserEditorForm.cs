using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Mock;

namespace StockExchange.Client.WinForms.Forms;

public class UserEditorForm : Form
{
    public UserRow Result { get; private set; } = new();

    public UserEditorForm()
    {
        Text = "Thêm người dùng";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(580, 600);
        MinimumSize = new Size(540, 560);
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
        header.Controls.Add(AppTheme.CreateLabel("Thông tin người dùng", 17F, FontStyle.Bold));
        header.Controls.Add(AppTheme.CreateLabel("Tài khoản và quyền truy cập hệ thống.", 9F, FontStyle.Regular, AppTheme.Muted));
        layout.Controls.Add(header, 0, 0);

        var fields = AppTheme.CreateFormGrid(150);
        var username = AppTheme.CreateTextBox();
        var email = AppTheme.CreateTextBox();
        var role = CreateCombo(["Member", "Admin"]);
        var status = CreateCombo(["Active", "Locked"]);
        var password = AppTheme.CreateTextBox();
        password.UseSystemPasswordChar = true;

        AppTheme.AddField(fields, "Tên đăng nhập", username);
        AppTheme.AddField(fields, "Email", email);
        AppTheme.AddField(fields, "Vai trò", role);
        AppTheme.AddField(fields, "Trạng thái", status);
        AppTheme.AddField(fields, "Mật khẩu tạm", password);
        layout.Controls.Add(fields, 0, 1);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Margin = new Padding(0, AppTheme.SpaceMd, 0, 0)
        };
        var save = AppTheme.CreateButton("Lưu người dùng");
        save.Width = 160;
        save.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(username.Text) || string.IsNullOrWhiteSpace(email.Text))
            {
                MessageBox.Show(this, "Username và email là bắt buộc.", "Template validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Result = new UserRow
            {
                Id = MockData.Users.Count == 0 ? 1 : MockData.Users.Max(user => user.Id) + 1,
                Username = username.Text.Trim(),
                Email = email.Text.Trim(),
                Role = role.Text,
                Status = status.Text,
                CreatedAt = DateTime.Now
            };
            DialogResult = DialogResult.OK;
            Close();
        };
        var cancel = AppTheme.CreateButton("Hủy", false);
        cancel.Width = 110;
        cancel.Click += (_, _) => Close();
        actions.Controls.Add(save);
        actions.Controls.Add(cancel);
        layout.Controls.Add(actions, 0, 2);

        AcceptButton = save;
        CancelButton = cancel;
        Shown += (_, _) => AppTheme.ApplyCommonStyles(this);
    }

    private static ComboBox CreateCombo(string[] values)
    {
        var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        combo.Items.AddRange(values);
        combo.SelectedIndex = 0;
        return combo;
    }
}
