using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Mock;

namespace StockExchange.Client.WinForms.Controls;

public sealed class StockSummaryControl : UserControl
{
    private readonly Label _symbol = CreateValueLabel(26F);
    private readonly Label _company = CreateTextLabel(AppTheme.Muted);
    private readonly Label _price = CreateValueLabel(26F);
    private readonly Label _change = CreateValueLabel(16F);
    private readonly Label _sector = CreateTextLabel(AppTheme.Muted);
    private readonly Label _volume = CreateValueLabel(18F);
    private readonly Label _open = CreateValueLabel(18F);
    private readonly Label _range = CreateValueLabel(18F);

    public StockSummaryControl()
    {
        Dock = DockStyle.Fill;
        BackColor = AppTheme.Surface;
        Margin = Padding.Empty;
        AutoScaleMode = AutoScaleMode.Font;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = Padding.Empty,
            Margin = Padding.Empty,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        layout.Controls.Add(BuildPrimaryCard(), 0, 0);
        layout.Controls.Add(BuildPriceCard(), 1, 0);
        layout.Controls.Add(BuildMetricCard("Khối lượng", _volume, AppTheme.Primary), 0, 1);

        var session = new EqualColumnPanel { Dock = DockStyle.Fill, Columns = 2, Gap = AppTheme.SpaceMd };
        session.Controls.Add(BuildMetricCard("Mở cửa", _open, AppTheme.Success));
        session.Controls.Add(BuildMetricCard("Biên độ", _range, AppTheme.Warning));
        layout.Controls.Add(session, 1, 1);
        Controls.Add(layout);
    }

    public void SetStock(StockRow stock)
    {
        _symbol.Text = stock.Symbol;
        _company.Text = stock.Company;
        _sector.Text = stock.Sector;
        _price.Text = $"{stock.Price:N2}";
        _change.Text = $"{stock.ChangePercent:+0.00;-0.00;0.00}%";
        _change.ForeColor = stock.ChangePercent >= 0 ? AppTheme.Success : AppTheme.Danger;
        _volume.Text = $"{stock.Volume:N0}";
        _open.Text = $"{stock.Price * 0.98m:N2}";
        _range.Text = $"{stock.Price * 0.96m:N2} - {stock.Price * 1.03m:N2}";
    }

    private Control BuildPrimaryCard()
    {
        var card = AppTheme.CreateCard(AppTheme.SpaceLg);
        card.Dock = DockStyle.Fill;
        var layout = CreateStack();
        layout.Controls.Add(_symbol);
        layout.Controls.Add(_company);
        layout.Controls.Add(_sector);
        card.Controls.Add(layout);
        return card;
    }

    private Control BuildPriceCard()
    {
        var card = AppTheme.CreateCard(AppTheme.SpaceLg);
        card.Dock = DockStyle.Fill;
        var layout = CreateStack();
        layout.Controls.Add(CreateTextLabel("Giá hiện tại", AppTheme.Muted));
        layout.Controls.Add(_price);
        layout.Controls.Add(_change);
        card.Controls.Add(layout);
        return card;
    }

    private static Control BuildMetricCard(string title, Label value, Color accent)
    {
        var card = AppTheme.CreateCard(AppTheme.SpaceLg);
        card.Dock = DockStyle.Fill;
        var layout = CreateStack();
        layout.Controls.Add(CreateTextLabel(title, AppTheme.Muted));
        value.ForeColor = accent;
        layout.Controls.Add(value);
        card.Controls.Add(layout);
        return card;
    }

    private static FlowLayoutPanel CreateStack() => new()
    {
        Dock = DockStyle.Fill,
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        AutoScroll = false,
        Margin = Padding.Empty,
        Padding = Padding.Empty
    };

    private static Label CreateTextLabel(Color color) => new()
    {
        AutoSize = true,
        Font = AppTheme.BodyFont,
        ForeColor = color,
        Margin = new Padding(0, 0, 0, AppTheme.SpaceSm)
    };

    private static Label CreateTextLabel(string text, Color color)
    {
        var label = CreateTextLabel(color);
        label.Text = text;
        return label;
    }

    private static Label CreateValueLabel(float size) => new()
    {
        AutoSize = true,
        Font = AppTheme.CreateFont(size, FontStyle.Bold),
        ForeColor = AppTheme.Text,
        Margin = new Padding(0, 0, 0, AppTheme.SpaceSm)
    };
}
