using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Mock;

namespace StockExchange.Client.WinForms.Controls;

public class WatchlistControl : UserControl
{
    private readonly Dictionary<StockRow, TableLayoutPanel> _rows = [];
    private readonly FlowLayoutPanel _list = new()
    {
        Dock = DockStyle.Fill,
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        AutoScroll = true,
        Padding = new Padding(AppTheme.SpaceXs),
        Margin = Padding.Empty
    };

    public event EventHandler<StockRow>? StockSelected;

    public WatchlistControl()
    {
        Dock = DockStyle.Fill;
        BackColor = AppTheme.Surface;
        Margin = Padding.Empty;
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_list);
        _list.SizeChanged += (_, _) => ResizeRows();
    }

    public void SetStocks(IEnumerable<StockRow> stocks)
    {
        var items = stocks.ToList();
        _list.Controls.Clear();
        _rows.Clear();

        if (items.Count == 0)
        {
            _list.Controls.Add(new Label
            {
                Text = "Watchlist chưa có cổ phiếu.",
                AutoSize = true,
                Font = AppTheme.BodyFont,
                ForeColor = AppTheme.Muted,
                Margin = new Padding(AppTheme.SpaceMd)
            });
            return;
        }

        foreach (var stock in items)
        {
            var row = new TableLayoutPanel
            {
                Width = GetRowWidth(),
                Height = 82,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = AppTheme.Surface,
                Margin = new Padding(0, 0, 0, AppTheme.SpaceSm),
                Padding = new Padding(AppTheme.SpaceMd, AppTheme.SpaceSm, AppTheme.SpaceMd, AppTheme.SpaceSm),
                Cursor = Cursors.Hand,
                Tag = stock
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            row.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            row.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            var symbol = new Label
            {
                Text = stock.Symbol,
                Font = AppTheme.CreateFont(16F, FontStyle.Bold),
                ForeColor = AppTheme.Text,
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            var price = new Label
            {
                Text = $"{stock.Price:N2}",
                Font = AppTheme.BodyFont,
                ForeColor = AppTheme.Text,
                AutoSize = true,
                Anchor = AnchorStyles.Right
            };
            var change = new Label
            {
                Text = $"{stock.ChangePercent:+0.00;-0.00;0.00}%",
                Font = AppTheme.SmallFont,
                ForeColor = stock.ChangePercent >= 0 ? AppTheme.Success : AppTheme.Danger,
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            row.Controls.Add(symbol, 0, 0);
            row.Controls.Add(price, 1, 0);
            row.Controls.Add(change, 0, 1);
            row.SetColumnSpan(change, 2);
            row.Click += HandleClick;
            foreach (Control child in row.Controls)
            {
                child.Cursor = Cursors.Hand;
                child.Click += (_, _) => SelectStock(stock);
            }
            _rows[stock] = row;
            _list.Controls.Add(row);
        }

        ResizeRows();
    }

    private int GetRowWidth() =>
        Math.Max(100, _list.ClientSize.Width - _list.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth - 4);

    private void ResizeRows()
    {
        var width = GetRowWidth();
        foreach (Control row in _list.Controls)
        {
            row.Width = width;
        }
    }

    private void HandleClick(object? sender, EventArgs eventArgs)
    {
        if (sender is Control { Tag: StockRow stock })
        {
            SelectStock(stock);
        }
    }

    public void SelectStock(StockRow stock)
    {
        foreach (var pair in _rows)
        {
            pair.Value.BackColor = ReferenceEquals(pair.Key, stock)
                ? AppTheme.PrimarySoft
                : AppTheme.Surface;
        }

        StockSelected?.Invoke(this, stock);
    }
}
