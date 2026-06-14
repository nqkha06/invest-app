using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Mock;

namespace StockExchange.Client.WinForms.Forms;

public class StockEditorForm : Form
{
    public StockRow Result { get; private set; } = new();

    public StockEditorForm(StockRow? stock = null)
    {
        var isEditing = stock is not null;

        Text = isEditing ? "Chỉnh sửa cổ phiếu" : "Thêm cổ phiếu";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(560, 570);
        MinimumSize = new Size(520, 540);
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
        header.Controls.Add(AppTheme.CreateLabel("Thông tin cổ phiếu", 17F, FontStyle.Bold));
        header.Controls.Add(AppTheme.CreateLabel("Thông tin tương ứng với model Stock phía server.", 9F, FontStyle.Regular, AppTheme.Muted));
        layout.Controls.Add(header, 0, 0);

        var fields = AppTheme.CreateFormGrid(150);
        var symbol = AppTheme.CreateTextBox();
        var company = AppTheme.CreateTextBox();
        var sector = AppTheme.CreateTextBox();
        var price = new NumericUpDown
        {
            DecimalPlaces = 2,
            Maximum = 100_000,
            ThousandsSeparator = true
        };
        var active = new CheckBox
        {
            Text = "Đang hoạt động",
            Checked = stock?.Active ?? true,
            AutoSize = true,
            ForeColor = AppTheme.Text,
            Anchor = AnchorStyles.Left
        };

        symbol.Text = stock?.Symbol ?? string.Empty;
        company.Text = stock?.Company ?? string.Empty;
        sector.Text = stock?.Sector ?? string.Empty;
        price.Value = Math.Clamp(stock?.Price ?? 0m, price.Minimum, price.Maximum);

        AppTheme.AddField(fields, "Mã cổ phiếu", symbol);
        AppTheme.AddField(fields, "Tên doanh nghiệp", company);
        AppTheme.AddField(fields, "Ngành", sector);
        AppTheme.AddField(fields, "Giá hiện tại", price);
        AppTheme.AddField(fields, "Trạng thái", active);
        layout.Controls.Add(fields, 0, 1);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Margin = new Padding(0, AppTheme.SpaceMd, 0, 0)
        };
        var save = AppTheme.CreateButton(isEditing ? "Lưu thay đổi" : "Lưu cổ phiếu");
        save.Width = 150;
        save.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(symbol.Text) || string.IsNullOrWhiteSpace(company.Text))
            {
                MessageBox.Show(this, "Mã và tên doanh nghiệp là bắt buộc.", "Dữ liệu chưa hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var normalizedSymbol = symbol.Text.Trim().ToUpperInvariant();
            Result = new StockRow
            {
                Id = stock?.Id ?? 0,
                Symbol = normalizedSymbol,
                Company = company.Text.Trim(),
                Sector = sector.Text.Trim(),
                Price = price.Value,
                ChangePercent = stock?.ChangePercent ?? 0,
                Volume = stock?.Volume ?? 0,
                Active = active.Checked
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
}
