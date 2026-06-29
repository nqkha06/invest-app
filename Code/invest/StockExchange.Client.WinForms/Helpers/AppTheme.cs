namespace StockExchange.Client.WinForms.Helpers;

using System.Drawing.Drawing2D;

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
    public const int RadiusSm = 8;
    public const int RadiusMd = 10;
    public const int RadiusLg = 14;

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
        var button = new RoundedButton
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
        button.Configure(RadiusMd, primary ? Primary : Border);
        button.FlatAppearance.BorderColor = primary ? Primary : Border;
        button.FlatAppearance.BorderSize = 1;
        return button;
    }

    public static Panel CreateCard(int padding = CardPadding)
    {
        var card = new RoundedPanel
        {
            BackColor = Surface,
            Padding = new Padding(padding),
            Margin = new Padding(SpaceSm)
        };
        card.Configure(RadiusLg, Border);
        return card;
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
            BorderStyle = BorderStyle.None,
            Font = BodyFont,
            MinimumSize = new Size(120, 0),
            BackColor = Surface,
            Margin = new Padding(0, SpaceXs, 0, SpaceMd)
        };
    }

    public static Control CreateInputFrame(TextBox input)
    {
        input.Dock = DockStyle.None;
        input.Anchor = AnchorStyles.Left | AnchorStyles.Right;

        var frame = new InputFrame(input)
        {
            Width = Math.Max(120, input.Width),
            Height = ControlHeight,
            MinimumSize = new Size(120, ControlHeight),
            Margin = input.Margin
        };
        input.Margin = Padding.Empty;
        return frame;
    }

    public static CheckBox CreateCheckBox(string text, bool isChecked = false)
    {
        return new StyledCheckBox
        {
            Text = text,
            Checked = isChecked,
            AutoSize = false,
            Width = 240,
            Height = ControlHeight,
            Font = BodyFont,
            ForeColor = Muted,
            BackColor = Color.Transparent,
            Margin = new Padding(0, SpaceXs, SpaceMd, SpaceMd),
            Cursor = Cursors.Hand
        };
    }

    public static Button CreateSidebarButton(string text)
    {
        var button = new RoundedButton
        {
            Text = text,
            TextAlign = ContentAlignment.MiddleLeft,
            Width = 200,
            Height = 48,
            BackColor = Sidebar,
            ForeColor = Color.FromArgb(203, 213, 225),
            Font = CreateFont(15F, FontStyle.Bold),
            Padding = new Padding(14, 0, 6, 0),
            Margin = new Padding(0, SpaceXs, 0, SpaceXs),
            Cursor = Cursors.Hand
        };
        button.Configure(RadiusMd, Sidebar);
        return button;
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

        var fieldControl = input is TextBox textBox ? CreateInputFrame(textBox) : input;
        fieldControl.Dock = DockStyle.Top;
        fieldControl.Margin = new Padding(0, SpaceXs, 0, SpaceMd);
        fieldControl.MinimumSize = new Size(160, ControlHeight);

        grid.Controls.Add(label, 0, row);
        grid.Controls.Add(fieldControl, 1, row);
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
                    combo.FlatStyle = FlatStyle.Flat;
                    combo.BackColor = Surface;
                    combo.MinimumSize = new Size(120, ControlHeight);
                    break;
                case NumericUpDown numeric:
                    numeric.Font = BodyFont;
                    numeric.BorderStyle = BorderStyle.FixedSingle;
                    numeric.BackColor = Surface;
                    numeric.MinimumSize = new Size(120, ControlHeight);
                    break;
                case TextBox textBox:
                    textBox.Font = BodyFont;
                    textBox.BorderStyle = BorderStyle.None;
                    textBox.MinimumSize = new Size(120, 0);
                    break;
                case CheckBox checkBox:
                    checkBox.Font = BodyFont;
                    checkBox.ForeColor = Muted;
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

    private static GraphicsPath CreateRoundPath(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        var diameter = Math.Max(1, radius * 2);
        var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

        path.AddArc(arc, 180, 90);
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }

    private sealed class RoundedButton : Button
    {
        private int _borderRadius = RadiusMd;
        private Color _borderColor = Border;

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
        }

        public void Configure(int borderRadius, Color borderColor)
        {
            _borderRadius = borderRadius;
            _borderColor = borderColor;
        }

        protected override void OnResize(EventArgs eventArgs)
        {
            base.OnResize(eventArgs);
            using var path = CreateRoundPath(new Rectangle(0, 0, Math.Max(1, Width - 1), Math.Max(1, Height - 1)), _borderRadius);
            Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs eventArgs)
        {
            eventArgs.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = CreateRoundPath(new Rectangle(0, 0, Width - 1, Height - 1), _borderRadius);
            using var fill = new SolidBrush(BackColor);
            using var border = new Pen(_borderColor);
            eventArgs.Graphics.FillPath(fill, path);
            eventArgs.Graphics.DrawPath(border, path);
            var textBounds = new Rectangle(
                Padding.Left,
                Padding.Top,
                Math.Max(0, Width - Padding.Horizontal),
                Math.Max(0, Height - Padding.Vertical));
            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            flags |= TextAlign switch
            {
                ContentAlignment.MiddleLeft or ContentAlignment.TopLeft or ContentAlignment.BottomLeft => TextFormatFlags.Left,
                ContentAlignment.MiddleRight or ContentAlignment.TopRight or ContentAlignment.BottomRight => TextFormatFlags.Right,
                _ => TextFormatFlags.HorizontalCenter
            };
            TextRenderer.DrawText(
                eventArgs.Graphics,
                Text,
                Font,
                textBounds,
                ForeColor,
                flags);
        }
    }

    private class RoundedPanel : Panel
    {
        private int _borderRadius = RadiusLg;
        private Color _borderColor = Border;

        public RoundedPanel()
        {
            DoubleBuffered = true;
        }

        public void Configure(int borderRadius, Color borderColor)
        {
            _borderRadius = borderRadius;
            _borderColor = borderColor;
        }

        protected override void OnResize(EventArgs eventArgs)
        {
            base.OnResize(eventArgs);
            using var path = CreateRoundPath(new Rectangle(0, 0, Math.Max(1, Width - 1), Math.Max(1, Height - 1)), _borderRadius);
            Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs eventArgs)
        {
            base.OnPaint(eventArgs);
            eventArgs.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = CreateRoundPath(new Rectangle(0, 0, Width - 1, Height - 1), _borderRadius);
            using var border = new Pen(_borderColor);
            eventArgs.Graphics.DrawPath(border, path);
        }
    }

    private sealed class InputFrame : Panel
    {
        private readonly TextBox _input;
        private Color _borderColor = Border;

        public InputFrame(TextBox input)
        {
            SetStyle(
                ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw,
                true);

            _input = input;
            BackColor = Surface;
            Padding = new Padding(SpaceMd, 0, SpaceMd, 0);
            Controls.Add(_input);
            _input.BorderStyle = BorderStyle.None;
            _input.BackColor = Surface;
            _input.Font = BodyFont;
            _input.Enter += (_, _) =>
            {
                _borderColor = Primary;
                Invalidate();
            };
            _input.Leave += (_, _) =>
            {
                _borderColor = Border;
                Invalidate();
            };
            LayoutInput();
        }

        protected override void OnResize(EventArgs eventArgs)
        {
            base.OnResize(eventArgs);
            LayoutInput();
        }

        protected override void OnControlAdded(ControlEventArgs eventArgs)
        {
            base.OnControlAdded(eventArgs);
            LayoutInput();
        }

        protected override void OnPaint(PaintEventArgs eventArgs)
        {
            eventArgs.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            eventArgs.Graphics.Clear(Parent?.BackColor ?? Surface);

            var borderBounds = new Rectangle(1, 1, Math.Max(1, Width - 3), Math.Max(1, Height - 3));
            using var path = CreateRoundPath(borderBounds, RadiusSm);
            using var fill = new SolidBrush(BackColor);
            using var border = new Pen(_borderColor, 1.2F);
            eventArgs.Graphics.FillPath(fill, path);
            eventArgs.Graphics.DrawPath(border, path);
        }

        private void LayoutInput()
        {
            if (_input.IsDisposed)
            {
                return;
            }

            var inputHeight = _input.PreferredHeight;
            _input.SetBounds(
                Padding.Left,
                Math.Max(1, (Height - inputHeight) / 2),
                Math.Max(1, Width - Padding.Horizontal),
                inputHeight);
        }
    }

    private sealed class StyledCheckBox : CheckBox
    {
        private bool _hovered;

        public StyledCheckBox()
        {
            SetStyle(
                ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw,
                true);
        }

        protected override void OnCheckedChanged(EventArgs eventArgs)
        {
            base.OnCheckedChanged(eventArgs);
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs eventArgs)
        {
            base.OnMouseEnter(eventArgs);
            _hovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs eventArgs)
        {
            base.OnMouseLeave(eventArgs);
            _hovered = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs eventArgs)
        {
            eventArgs.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            eventArgs.Graphics.Clear(Parent?.BackColor ?? Surface);

            const int boxSize = 28;
            var boxTop = Math.Max(0, (Height - boxSize) / 2);
            var boxBounds = new Rectangle(0, boxTop, boxSize, boxSize);
            var borderColor = Checked ? Primary : (_hovered ? Color.FromArgb(148, 163, 184) : Border);
            var fillColor = Checked ? Primary : Surface;

            using var boxPath = CreateRoundPath(boxBounds, RadiusSm);
            using var fill = new SolidBrush(fillColor);
            using var border = new Pen(borderColor, 1.4F);
            eventArgs.Graphics.FillPath(fill, boxPath);
            eventArgs.Graphics.DrawPath(border, boxPath);

            if (Checked)
            {
                using var checkPen = new Pen(Color.White, 2.4F)
                {
                    StartCap = LineCap.Round,
                    EndCap = LineCap.Round,
                    LineJoin = LineJoin.Round
                };
                var points = new[]
                {
                    new PointF(boxBounds.Left + 7, boxBounds.Top + 14),
                    new PointF(boxBounds.Left + 12, boxBounds.Top + 19),
                    new PointF(boxBounds.Left + 21, boxBounds.Top + 9)
                };
                eventArgs.Graphics.DrawLines(checkPen, points);
            }

            var textBounds = new Rectangle(
                boxBounds.Right + SpaceSm,
                0,
                Math.Max(0, Width - boxBounds.Right - SpaceSm),
                Height);
            TextRenderer.DrawText(
                eventArgs.Graphics,
                Text,
                Font,
                textBounds,
                ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }
    }

    public static void ShowTemplateNotice(IWin32Window owner, string action, string message = "")
    {
        MessageBox.Show(
            owner,
            $"{action}\n\n{message ?? "Chức năng đang được phát triển ..!"}",
            "Thông báo",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
