using System.ComponentModel;
using StockExchange.Client.WinForms.Controls;
using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Models;
using StockExchange.Shared.DTOs;

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
        _detailPrice = new Label
        {
            Text = $"{stock.Price:N2}",
            AutoSize = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.BottomLeft,
            Font = AppTheme.CreateFont(28F, FontStyle.Bold),
            ForeColor = AppTheme.Text
        };
        overviewLayout.Controls.Add(_detailPrice, 1, 0);
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
        _detailChange = new Label
        {
            Text = $"{stock.ChangePercent:+0.00;-0.00;0.00}%",
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft,
            Font = AppTheme.CreateFont(16F, FontStyle.Bold),
            ForeColor = stock.ChangePercent >= 0 ? AppTheme.Success : AppTheme.Danger
        };
        overviewLayout.Controls.Add(_detailChange, 1, 1);
        var watch = AppTheme.CreateButton(IsInWatchlist(stock) ? "Đã theo dõi" : "+ Watchlist");
        watch.Anchor = AnchorStyles.Right;
        watch.Margin = new Padding(AppTheme.SpaceLg, 0, 0, 0);
        watch.Click += async (_, _) =>
        {
            watch.Enabled = false;
            try
            {
                var shouldWatch = !IsInWatchlist(stock);
                await SetWatchlistAsync(stock, shouldWatch);
                watch.Text = shouldWatch ? "Đã theo dõi" : "+ Watchlist";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Không thể cập nhật watchlist",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                watch.Enabled = true;
            }
        };
        overviewLayout.Controls.Add(watch, 2, 0);
        overviewLayout.SetRowSpan(watch, 2);
        overview.Controls.Add(overviewLayout);
        page.Controls.Add(overview, 0, 0);

        var candleChart = new CandlestickChartControl { Dock = DockStyle.Fill };
        _detailChart = candleChart;
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
        chartToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 720F));
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

        async Task LoadCandlesAsync(Button activeButton, string intervalCode, int count, int visibleCount, TimeSpan interval)
        {
            _detailCandleInterval = interval;
            _detailChartInterval = intervalCode;
            _detailCandleMaxCount = count;
            _detailCandles.Clear();
            try
            {
                var history = await _chartService.GetHistoryAsync(stock.Symbol, intervalCode);
                _detailCandles.AddRange(history.Select(ToCandlePoint));
            }
            catch
            {
                _detailCandles.Clear();
            }

            while (_detailCandles.Count > count)
            {
                _detailCandles.RemoveAt(0);
            }

            candleChart.SetCandles(_detailCandles, visibleCount);
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
                     (Text: "1p", Count: 240, Visible: 120, Interval: TimeSpan.FromMinutes(1)),
                     (Text: "5p", Count: 240, Visible: 120, Interval: TimeSpan.FromMinutes(5)),
                     (Text: "15p", Count: 192, Visible: 96, Interval: TimeSpan.FromMinutes(15)),
                     (Text: "30p", Count: 192, Visible: 96, Interval: TimeSpan.FromMinutes(30)),
                     (Text: "1h", Count: 168, Visible: 84, Interval: TimeSpan.FromHours(1)),
                     (Text: "Ngày", Count: 180, Visible: 90, Interval: TimeSpan.FromDays(1)),
                     (Text: "Tuần", Count: 104, Visible: 52, Interval: TimeSpan.FromDays(7)),
                     (Text: "Tháng", Count: 60, Visible: 36, Interval: TimeSpan.FromDays(30))
                 })
        {
            var button = AppTheme.CreateButton(option.Text, false);
            button.Width = option.Text.Length > 3 ? 66 : 54;
            button.Height = 36;
            button.Margin = new Padding(AppTheme.SpaceXs, 0, 0, 0);
            var intervalCode = option.Text switch
            {
                "1p" => "1MIN",
                "5p" => "5MIN",
                "15p" => "15MIN",
                "30p" => "30MIN",
                "1h" => "1H",
                "Ngày" => "1D",
                "Tuần" => "1W",
                "Tháng" => "1M",
                _ => "5MIN"
            };
            button.Click += async (_, _) => await LoadCandlesAsync(button, intervalCode, option.Count, option.Visible, option.Interval);
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
        _ = LoadCandlesAsync(intervalButtons[0], "1MIN", 240, 120, TimeSpan.FromMinutes(1));

        var stockSummary = new StockSummaryControl();
        stockSummary.SetStock(stock);
        _detailSummary = stockSummary;
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

    private Control BuildEmptyDetailPage()
    {
        return new Label
        {
            Text = "Chọn một cổ phiếu từ bảng giá để xem chi tiết.",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = AppTheme.BodyFont,
            ForeColor = AppTheme.Muted
        };
    }

    private static CandlePoint ToCandlePoint(PriceHistoryDto point)
    {
        var time = point.Timestamp.TimeOfDay == TimeSpan.Zero
            ? DateTime.SpecifyKind(point.Timestamp.Date, DateTimeKind.Unspecified)
            : point.Timestamp.ToLocalTime();

        return new CandlePoint
        {
            Time = time,
            Open = point.Open,
            High = point.High,
            Low = point.Low,
            Close = point.Close,
            Volume = point.Volume
        };
    }

}
