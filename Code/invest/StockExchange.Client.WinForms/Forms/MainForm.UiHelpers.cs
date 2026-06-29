using System.ComponentModel;
using StockExchange.Client.WinForms.Controls;
using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Models;

namespace StockExchange.Client.WinForms.Forms;

public partial class MainForm : Form
{
    private static Panel BuildStatCard(string title, string value, string note, Color accent)
    {
        var card = AppTheme.CreateCard(18);
        card.Dock = DockStyle.Fill;
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            Margin = Padding.Empty,
            Padding = new Padding(4, 0, 0, 0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 8));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 44F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
        var stripe = new Panel { Dock = DockStyle.Fill, BackColor = accent, Margin = new Padding(0, 0, 4, 0) };
        layout.Controls.Add(stripe, 0, 0);
        layout.SetRowSpan(stripe, 3);
        layout.Controls.Add(new Label
        {
            Text = title,
            AutoSize = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = AppTheme.SmallFont,
            ForeColor = AppTheme.Muted,
            Margin = new Padding(10, 0, 0, 0)
        }, 1, 0);
        layout.Controls.Add(new Label
        {
            Text = value,
            AutoSize = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = AppTheme.CreateFont(24F, FontStyle.Bold),
            ForeColor = AppTheme.Text,
            Margin = new Padding(10, 0, 0, 0)
        }, 1, 1);
        layout.Controls.Add(new Label
        {
            Text = note,
            AutoSize = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = AppTheme.SmallFont,
            ForeColor = accent,
            Margin = new Padding(10, 0, 0, 0)
        }, 1, 2);
        card.Controls.Add(layout);
        return card;
    }

    private static Control BuildHealthRow(string name, string status)
    {
        var row = new TableLayoutPanel
        {
            Width = 300,
            Height = 46,
            ColumnCount = 2,
            Margin = new Padding(0, 6, 0, 0)
        };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
        row.Controls.Add(new Label
        {
            Text = name,
            AutoSize = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = AppTheme.Text,
            Font = AppTheme.BodyFont
        }, 0, 0);
        row.Controls.Add(new Label
        {
            Text = status,
            AutoSize = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = status == "Sẵn sàng" ? AppTheme.Success : AppTheme.Warning,
            Font = AppTheme.CreateFont(13F, FontStyle.Bold)
        }, 1, 0);
        return row;
    }

    private static TableLayoutPanel BuildToolbar(string placeholder, string actionText, out TextBox search, out Button action)
    {
        var toolbar = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Background,
            ColumnCount = 3,
            RowCount = 1,
            Padding = new Padding(0, AppTheme.SpaceSm, 0, AppTheme.SpaceSm)
        };
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        search = AppTheme.CreateTextBox(placeholder);
        var searchFrame = AppTheme.CreateInputFrame(search);
        searchFrame.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        searchFrame.Margin = new Padding(0, 0, AppTheme.SpaceMd, 0);
        var actionButton = AppTheme.CreateButton(actionText);
        actionButton.Anchor = AnchorStyles.Right;
        actionButton.Margin = Padding.Empty;
        action = actionButton;
        toolbar.Controls.Add(searchFrame, 0, 0);
        toolbar.Controls.Add(actionButton, 2, 0);
        return toolbar;
    }

    private static Panel WrapGrid(DataGridView grid, string title) => WrapControl(grid, title);

    private static Panel WrapControl(Control control, string title)
    {
        var card = AppTheme.CreateCard();
        card.Dock = DockStyle.Fill;
        var layout = AppTheme.CreatePage(2);
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.BackColor = AppTheme.Surface;
        layout.Controls.Add(new Label
        {
            Text = title,
            AutoSize = true,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = AppTheme.HeadingFont,
            ForeColor = AppTheme.Text,
            Margin = Padding.Empty
        }, 0, 0);
        control.Dock = DockStyle.Fill;
        control.Margin = Padding.Empty;
        layout.Controls.Add(control, 0, 1);
        card.Controls.Add(layout);
        return card;
    }

    private static TextBox AddReadOnlyField(FlowLayoutPanel form, string label)
    {
        var input = AddTextField(form, label, "");
        input.ReadOnly = true;
        input.BackColor = Color.FromArgb(248, 250, 252);
        return input;
    }

    private static TextBox AddTextField(FlowLayoutPanel form, string label, string value)
    {
        form.Controls.Add(AppTheme.CreateLabel(label, 9.5F, FontStyle.Bold));
        var input = AppTheme.CreateTextBox();
        input.Width = 480;
        input.Text = value;
        form.Controls.Add(AppTheme.CreateInputFrame(input));
        return input;
    }

    private static ComboBox AddComboField(FlowLayoutPanel form, string label, string[] values)
    {
        form.Controls.Add(AppTheme.CreateLabel(label, 9.5F, FontStyle.Bold));
        var combo = new ComboBox
        {
            Width = 320,
            Height = 36,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = AppTheme.BodyFont,
            Margin = new Padding(3, 6, 3, 12)
        };
        combo.Items.AddRange(values);
        combo.SelectedIndex = 0;
        form.Controls.Add(combo);
        return combo;
    }

    private static NumericUpDown AddNumericField(
        FlowLayoutPanel form, string label, int decimals, decimal value, decimal maximum = 1)
    {
        form.Controls.Add(AppTheme.CreateLabel(label, 9.5F, FontStyle.Bold));
        var input = new NumericUpDown
        {
            Width = 320,
            Height = 36,
            DecimalPlaces = decimals,
            Increment = decimals >= 3 ? 0.001m : 0.01m,
            Minimum = 0,
            Maximum = maximum,
            Value = Math.Min(value, maximum),
            Font = AppTheme.BodyFont,
            Margin = new Padding(3, 6, 3, 12)
        };
        form.Controls.Add(input);
        return input;
    }

    private static decimal Clamp(NumericUpDown input, decimal value) =>
        Math.Min(input.Maximum, Math.Max(input.Minimum, value));
}
