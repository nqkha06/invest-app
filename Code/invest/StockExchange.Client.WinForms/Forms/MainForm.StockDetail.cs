using System.ComponentModel;
using StockExchange.Client.WinForms.Controls;
using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Mock;

namespace StockExchange.Client.WinForms.Forms;

public partial class MainForm : Form
{
    private Control BuildStockDetailPage(StockRow stock)
    {
        var scrollHost = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = AppTheme.Background,
            Padding = Padding.Empty
        };
        var page = AppTheme.CreatePage(4);
        page.Dock = DockStyle.Top;
        page.Height = 1130;
        page.AutoScroll = false;
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 460));
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 360));
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));
        scrollHost.Controls.Add(page);

        var overview = AppTheme.CreateCard();
        overview.Dock = DockStyle.Fill;
        var overviewLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 2,
            Margin = Padding.Empty
        };
        overviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
        overviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
        overviewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26F));
        overviewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 58F));
        overviewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 42F));
        overviewLayout.Controls.Add(new Label
        {
            Text = stock.Symbol,
            AutoSize = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.BottomLeft,
            Font = AppTheme.CreateFont(28F, FontStyle.Bold),
            ForeColor = AppTheme.Text
        }, 0, 0);
        overviewLayout.Controls.Add(new Label
        {
            Text = $"{stock.Price:N2}",
            AutoSize = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.BottomLeft,
            Font = AppTheme.CreateFont(28F, FontStyle.Bold),
            ForeColor = AppTheme.Text
        }, 1, 0);
        overviewLayout.Controls.Add(new Label
        {
            Text = $"{stock.Company} • {stock.Sector}",
            AutoSize = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.TopLeft,
            Font = AppTheme.BodyFont,
            ForeColor = AppTheme.Muted
        }, 0, 1);
        overviewLayout.Controls.Add(new Label
        {
            Text = $"{stock.ChangePercent:+0.00;-0.00;0.00}%",
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft,
            Font = AppTheme.CreateFont(16F, FontStyle.Bold),
            ForeColor = stock.ChangePercent >= 0 ? AppTheme.Success : AppTheme.Danger
        }, 1, 1);
        var watch = AppTheme.CreateButton(MockData.Watchlist.Contains(stock) ? "Đã theo dõi" : "+ Watchlist");
        watch.Anchor = AnchorStyles.Right;
        watch.Margin = new Padding(AppTheme.SpaceLg, 0, 0, 0);
        watch.Click += (_, _) =>
        {
            if (MockData.Watchlist.Contains(stock))
            {
                MockData.Watchlist.Remove(stock);
                watch.Text = "+ Watchlist";
            }
            else
            {
                MockData.Watchlist.Add(stock);
                watch.Text = "Đã theo dõi";
            }
        };
        overviewLayout.Controls.Add(watch, 2, 0);
        overviewLayout.SetRowSpan(watch, 2);
        overview.Controls.Add(overviewLayout);
        page.Controls.Add(overview, 0, 0);

        var candleChart = new CandlestickChartControl { Dock = DockStyle.Fill };
        var chartCard = AppTheme.CreateCard();
        chartCard.Dock = DockStyle.Fill;
        var chartLayout = AppTheme.CreatePage(2);
        chartLayout.BackColor = AppTheme.Surface;
        chartLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        chartLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var chartToolbar = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty
        };
        chartToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        chartToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 470F));
        chartToolbar.Controls.Add(
            AppTheme.CreateLabel("Biểu đồ nến OHLCV • Cuộn để zoom, kéo ngang để pan", 14F, FontStyle.Bold),
            0,
            0);

        var intervals = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Anchor = AnchorStyles.Right,
            Margin = Padding.Empty
        };
        var intervalButtons = new List<Button>();

        void LoadCandles(Button activeButton, int count, int visibleCount, TimeSpan interval)
        {
            candleChart.SetCandles(MockData.BuildCandles(stock, count, interval), visibleCount);
            foreach (var button in intervalButtons)
            {
                var active = ReferenceEquals(button, activeButton);
                button.BackColor = active ? AppTheme.Primary : AppTheme.Surface;
                button.ForeColor = active ? Color.White : AppTheme.Text;
                button.FlatAppearance.BorderColor = active ? AppTheme.Primary : AppTheme.Border;
            }
        }

        foreach (var option in new[]
                 {
                     (Text: "1D", Count: 96, Visible: 72, Interval: TimeSpan.FromMinutes(5)),
                     (Text: "1W", Count: 120, Visible: 80, Interval: TimeSpan.FromHours(1)),
                     (Text: "1M", Count: 90, Visible: 65, Interval: TimeSpan.FromHours(8)),
                     (Text: "3M", Count: 120, Visible: 75, Interval: TimeSpan.FromDays(1))
                 })
        {
            var button = AppTheme.CreateButton(option.Text, false);
            button.Width = 54;
            button.Height = 36;
            button.Margin = new Padding(AppTheme.SpaceXs, 0, 0, 0);
            button.Click += (_, _) => LoadCandles(button, option.Count, option.Visible, option.Interval);
            intervalButtons.Add(button);
            intervals.Controls.Add(button);
        }

        var zoomOut = AppTheme.CreateButton("−", false);
        zoomOut.Width = 42;
        zoomOut.Height = 36;
        zoomOut.Margin = new Padding(AppTheme.SpaceMd, 0, 0, 0);
        zoomOut.Click += (_, _) => candleChart.ZoomOut();
        intervals.Controls.Add(zoomOut);

        var zoomIn = AppTheme.CreateButton("+", false);
        zoomIn.Width = 42;
        zoomIn.Height = 36;
        zoomIn.Margin = new Padding(AppTheme.SpaceXs, 0, 0, 0);
        zoomIn.Click += (_, _) => candleChart.ZoomIn();
        intervals.Controls.Add(zoomIn);

        var resetZoom = AppTheme.CreateButton("Reset", false);
        resetZoom.Width = 68;
        resetZoom.Height = 36;
        resetZoom.Margin = new Padding(AppTheme.SpaceXs, 0, 0, 0);
        resetZoom.Click += (_, _) => candleChart.ResetView();
        intervals.Controls.Add(resetZoom);

        chartToolbar.Controls.Add(intervals, 1, 0);
        chartLayout.Controls.Add(chartToolbar, 0, 0);
        chartLayout.Controls.Add(candleChart, 0, 1);
        chartCard.Controls.Add(chartLayout);
        page.Controls.Add(chartCard, 0, 1);
        LoadCandles(intervalButtons[0], 96, 72, TimeSpan.FromMinutes(5));

        var stockSummary = new StockSummaryControl();
        stockSummary.SetStock(stock);
        page.Controls.Add(WrapControl(stockSummary, "Tóm tắt giao dịch"), 0, 2);

        var metrics = new EqualColumnPanel
        {
            Dock = DockStyle.Fill,
            Columns = 4,
            Gap = 12
        };
        metrics.Controls.Add(BuildStatCard("Mở cửa", $"{stock.Price * 0.98m:N2}", "Phiên hôm nay", AppTheme.Primary));
        metrics.Controls.Add(BuildStatCard("Cao nhất", $"{stock.Price * 1.03m:N2}", "Trong phiên", AppTheme.Success));
        metrics.Controls.Add(BuildStatCard("Thấp nhất", $"{stock.Price * 0.96m:N2}", "Trong phiên", AppTheme.Danger));
        metrics.Controls.Add(BuildStatCard("Khối lượng", $"{stock.Volume:N0}", "Cổ phiếu", AppTheme.Warning));
        page.Controls.Add(metrics, 0, 3);
        return scrollHost;
    }

}
