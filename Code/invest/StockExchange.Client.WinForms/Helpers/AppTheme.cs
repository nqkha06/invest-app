namespace StockExchange.Client.WinForms.Helpers;

public static class AppTheme
{
    public const int SpaceXs = 4;
    public const int SpaceSm = 8;
    public const int SpaceMd = 12;
    public const int SpaceLg = 16;
    public const int SpaceXl = 24;
    public const int ControlHeight = 42;
    public const int ButtonHeight = 44;
    public const int CardPadding = 16;

    public static readonly Color Background = Color.FromArgb(245, 247, 251);
    public static readonly Color Surface = Color.White;
    public static readonly Color Sidebar = Color.FromArgb(15, 23, 42);
    public static readonly Color Primary = Color.FromArgb(37, 99, 235);
    public static readonly Color PrimarySoft = Color.FromArgb(219, 234, 254);
    public static readonly Color Success = Color.FromArgb(22, 163, 74);
    public static readonly Color Danger = Color.FromArgb(220, 38, 38);
    public static readonly Color Warning = Color.FromArgb(249, 115, 22);
    public static readonly Color Text = Color.FromArgb(17, 24, 39);
    public static readonly Color Muted = Color.FromArgb(100, 116, 139);
    public static readonly Color Border = Color.FromArgb(229, 231, 235);

    public static readonly Font TitleFont = CreateFont(27F, FontStyle.Bold);
    public static readonly Font HeadingFont = CreateFont(18F, FontStyle.Bold);
    public static readonly Font BodyFont = CreateFont(15F);
    public static readonly Font SmallFont = CreateFont(13F);

    public static Font CreateFont(float pixelSize, FontStyle style = FontStyle.Regular) =>
        new("Segoe UI", pixelSize, style, GraphicsUnit.Pixel);

    public static Button CreateButton(string text, bool primary = true)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = false,
            AutoEllipsis = true,
            Height = ButtonHeight,
            Width = 136,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = CreateFont(14F, FontStyle.Bold),
            BackColor = primary ? Primary : Surface,
            ForeColor = primary ? Color.White : Text,
            Margin = new Padding(SpaceSm)
        };
        button.FlatAppearance.BorderColor = primary ? Primary : Border;
        button.FlatAppearance.BorderSize = 1;
        return button;
    }

    public static Panel CreateCard(int padding = CardPadding)
    {
        return new Panel
        {
            BackColor = Surface,
            Padding = new Padding(padding),
            Margin = new Padding(SpaceSm)
        };
    }

    public static Label CreateLabel(
        string text,
        float size = 10F,
        FontStyle style = FontStyle.Regular,
        Color? color = null)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            Font = CreateFont(size, style),
            ForeColor = color ?? Text,
            Margin = new Padding(0, 0, 0, SpaceSm),
            UseCompatibleTextRendering = true
        };
    }

    public static TextBox CreateTextBox(string placeholder = "")
    {
        return new TextBox
        {
            PlaceholderText = placeholder,
            BorderStyle = BorderStyle.FixedSingle,
            Font = BodyFont,
            MinimumSize = new Size(120, ControlHeight),
            Margin = new Padding(0, SpaceXs, 0, SpaceMd)
        };
    }

    public static DataGridView CreateGrid()
    {
        var grid = new BufferedDataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Surface,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoGenerateColumns = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            Font = BodyFont,
            GridColor = Border,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        };
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(248, 250, 252),
            ForeColor = Text,
            Font = CreateFont(14F, FontStyle.Bold),
            Padding = new Padding(SpaceSm),
            SelectionBackColor = Color.FromArgb(248, 250, 252),
            SelectionForeColor = Text
        };
        grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Surface,
            ForeColor = Text,
            SelectionBackColor = PrimarySoft,
            SelectionForeColor = Text,
            Padding = new Padding(SpaceSm, SpaceXs, SpaceSm, SpaceXs)
        };
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersHeight = 44;
        grid.RowTemplate.Height = 40;
        return grid;
    }

    private sealed class BufferedDataGridView : DataGridView
    {
        public BufferedDataGridView()
        {
            DoubleBuffered = true;
        }
    }

    public static TableLayoutPanel CreatePage(int rows)
    {
        var page = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = rows,
            BackColor = Background,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        return page;
    }

    public static TableLayoutPanel CreateFormGrid(int labelWidth = 140)
    {
        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            ColumnCount = 2,
            RowCount = 0,
            Padding = Padding.Empty,
            Margin = Padding.Empty
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, labelWidth));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        return grid;
    }

    public static void AddField(TableLayoutPanel grid, string labelText, Control input)
    {
        var row = grid.RowCount++;
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = CreateLabel(labelText, 9.5F, FontStyle.Bold);
        label.Anchor = AnchorStyles.Left;
        label.Margin = new Padding(0, SpaceSm, SpaceMd, SpaceMd);

        input.Dock = DockStyle.Top;
        input.Margin = new Padding(0, SpaceXs, 0, SpaceMd);
        input.MinimumSize = new Size(160, ControlHeight);
        grid.Controls.Add(label, 0, row);
        grid.Controls.Add(input, 1, row);
    }

    public static void ApplyCommonStyles(Control root)
    {
        foreach (Control control in root.Controls)
        {
            switch (control)
            {
                case DataGridView grid:
                    StyleGrid(grid);
                    break;
                case ComboBox combo:
                    combo.Font = BodyFont;
                    combo.IntegralHeight = false;
                    combo.MinimumSize = new Size(120, ControlHeight);
                    break;
                case NumericUpDown numeric:
                    numeric.Font = BodyFont;
                    numeric.MinimumSize = new Size(120, ControlHeight);
                    break;
                case TextBox textBox:
                    textBox.Font = BodyFont;
                    textBox.MinimumSize = new Size(120, ControlHeight);
                    break;
                case TabControl tabs:
                    tabs.Padding = new Point(SpaceMd, SpaceSm);
                    break;
                case GroupBox group:
                    group.Padding = new Padding(SpaceMd);
                    break;
            }

            ApplyCommonStyles(control);
        }
    }

    private static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Surface;
        grid.BorderStyle = BorderStyle.None;
        grid.RowHeadersVisible = false;
        grid.GridColor = Border;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        grid.ColumnHeadersHeight = Math.Max(44, TextRenderer.MeasureText("Ag", grid.ColumnHeadersDefaultCellStyle.Font).Height + 18);
        grid.RowTemplate.Height = Math.Max(40, TextRenderer.MeasureText("Ag", grid.Font).Height + 16);
        foreach (DataGridViewRow row in grid.Rows)
        {
            row.Height = grid.RowTemplate.Height;
        }
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.Font = CreateFont(14F, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.SelectionBackColor = PrimarySoft;
        grid.DefaultCellStyle.SelectionForeColor = Text;
    }

    public static void ShowTemplateNotice(IWin32Window owner, string action)
    {
        MessageBox.Show(
            owner,
            $"{action}\n\nĐây là thao tác minh họa. Chưa kết nối server.",
            "UI Template",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
