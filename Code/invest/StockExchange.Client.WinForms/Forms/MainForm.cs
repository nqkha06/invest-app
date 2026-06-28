using System.ComponentModel;
using StockExchange.Client.WinForms.Controls;
using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Mock;
using StockExchange.Client.WinForms.Services;
using StockExchange.Shared.DTOs;
using StockExchange.Shared.Models;

namespace StockExchange.Client.WinForms.Forms;

public partial class MainForm : Form
{
    private string _username;
    private readonly bool _isAdmin;
    private readonly AuthClientService _authService;
    private readonly StockClientService _stockService;
    private readonly ChartClientService _chartService;
    private readonly ClientConnectionService _connection;
    private UserProfileDto _profile;
    private readonly Label _accountName = new();
    private readonly Label _accountAvatar = new();
    private readonly Panel _content = new() { Dock = DockStyle.Fill, BackColor = AppTheme.Background, Padding = new Padding(AppTheme.SpaceXl) };
    private readonly Label _pageTitle = AppTheme.CreateLabel("", 18F, FontStyle.Bold);
    private readonly Dictionary<string, Button> _navButtons = [];
    private readonly BindingList<StockRow> _stocks = [];
    private readonly BindingList<StockRow> _marketRows = [];
    private readonly Dictionary<long, StockRow> _stockById = [];
    private readonly object _pendingPriceUpdateLock = new();
    private readonly Dictionary<long, StockPriceUpdateDto> _pendingPriceUpdates = [];
    private readonly System.Windows.Forms.Timer _priceUpdateTimer = new() { Interval = 1000 };
    private int _refreshStocksInFlight;
    private StockRow _selectedStock = MockData.Stocks[0];
    private string _currentPage = string.Empty;
    private string _marketSearch = string.Empty;
    private StockTableControl? _marketTable;
    private WatchlistControl? _watchlistControl;
    private StockRow? _watchlistSelectedStock;
    private StockSummaryControl? _watchlistSummary;
    private Label? _detailPrice;
    private Label? _detailChange;
    private StockSummaryControl? _detailSummary;
    private CandlestickChartControl? _detailChart;
    private readonly List<CandlePoint> _detailCandles = [];
    private TimeSpan _detailCandleInterval = TimeSpan.FromMinutes(5);
    private string _detailChartInterval = "5MIN";
    private int _detailCandleMaxCount = 96;

    public MainForm(
        LoginResponseDto login,
        AuthClientService authService,
        StockClientService stockService,
        ChartClientService chartService,
        ClientConnectionService connection)
    {
        _username = login.Username ?? "User"; 
        _isAdmin = string.Equals(login.Role, "Admin", StringComparison.OrdinalIgnoreCase);
        _authService = authService;
        _stockService = stockService;
        _chartService = chartService;
        _connection = connection;
        _connection.StockPriceUpdated += HandleStockPriceUpdated;
        _priceUpdateTimer.Tick += (_, _) => FlushPendingPriceUpdates();
        _profile = new UserProfileDto
        {
            UserId = login.UserId,
            Username = login.Username ?? "User",
            Email = login.Email ?? "no-email@example.com",
            Role = login.Role ?? "Member",
            IsActive = true
        };

        Text = $"Invest App - {(_isAdmin ? "Admin" : "Thành viên")}";
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(900, 620);
        BackColor = AppTheme.Background;
        Font = AppTheme.BodyFont;
        AutoScaleMode = AutoScaleMode.Font;

        var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240));
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        Controls.Add(shell);

        shell.Controls.Add(BuildSidebar(), 0, 0);
        shell.Controls.Add(BuildWorkspace(), 1, 0);
        Navigate(_isAdmin ? "Tổng quan" : "Thị trường");
        Shown += (_, _) => AppTheme.ApplyCommonStyles(this);
        FormClosed += (_, _) =>
        {
            _connection.StockPriceUpdated -= HandleStockPriceUpdated;
            _priceUpdateTimer.Stop();
            _priceUpdateTimer.Dispose();
        };
    }

    private Control BuildSidebar()
    {
        var sidebar = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Sidebar, Padding = new Padding(16, 20, 16, 16) };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 108));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 128));
        sidebar.Controls.Add(layout);

        var brand = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        brand.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        brand.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        brand.Controls.Add(new Label
        {
            Text = "INVEST",
            AutoSize = true,
            Font = AppTheme.CreateFont(26F, FontStyle.Bold),
            ForeColor = Color.White,
            Margin = new Padding(8, 4, 0, 2)
        }, 0, 0);
        brand.Controls.Add(new Label
        {
            Text = _isAdmin ? "QUẢN TRỊ" : "THỊ TRƯỜNG",
            AutoSize = true,
            Font = AppTheme.CreateFont(12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(96, 165, 250),
            Margin = new Padding(10, 0, 0, 0)
        }, 0, 1);
        layout.Controls.Add(brand, 0, 0);

        var menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };
        var items = _isAdmin
            ? new[] { "Tổng quan", "Người dùng", "Cổ phiếu", "Mô phỏng" }
            : new[] { "Thị trường", "Chi tiết stock", "Watchlist", "Hồ sơ" };
        foreach (var item in items)
        {
            menu.Controls.Add(CreateNavButton(item));
        }
        layout.Controls.Add(menu, 0, 1);

        var account = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            Padding = new Padding(4, 8, 0, 0)
        };
        account.RowStyles.Add(new RowStyle(SizeType.Percent, 34F));
        account.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
        account.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
        account.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52));
        account.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _accountAvatar.Text = !string.IsNullOrEmpty(_username) ? _username[..1].ToUpperInvariant() : "?";
        
        _accountAvatar.AutoSize = false;
        _accountAvatar.Size = new Size(42, 42);
        _accountAvatar.TextAlign = ContentAlignment.MiddleCenter;
        _accountAvatar.BackColor = AppTheme.Primary;
        _accountAvatar.ForeColor = Color.White;
        _accountAvatar.Font = AppTheme.CreateFont(17F, FontStyle.Bold);
        _accountAvatar.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        account.Controls.Add(_accountAvatar, 0, 0);
        account.SetRowSpan(account.Controls[^1], 3);
        _accountName.Text = _username;
        _accountName.AutoSize = false;
        _accountName.Dock = DockStyle.Fill;
        _accountName.AutoEllipsis = true;
        _accountName.ForeColor = Color.White;
        _accountName.Font = AppTheme.CreateFont(15F, FontStyle.Bold);
        account.Controls.Add(_accountName, 1, 0);
        account.Controls.Add(new Label
        {
            Text = _isAdmin ? "Quản trị viên" : "Thành viên",
            AutoSize = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            ForeColor = Color.FromArgb(148, 163, 184),
            Font = AppTheme.SmallFont
        }, 1, 1);
        var logout = new LinkLabel
        {
            Text = "Đăng xuất",
            AutoSize = true,
            LinkColor = Color.FromArgb(147, 197, 253),
            Font = AppTheme.SmallFont,
            Anchor = AnchorStyles.Left
        };
        logout.Click += (_, _) => Close();
        account.Controls.Add(logout, 1, 2);
        layout.Controls.Add(account, 0, 2);
        return sidebar;
    }

    private Button CreateNavButton(string text)
    {
        var button = new Button
        {
            Text = text,
            TextAlign = ContentAlignment.MiddleLeft,
            Width = 200,
            Height = 48,
            FlatStyle = FlatStyle.Flat,
            BackColor = AppTheme.Sidebar,
            ForeColor = Color.FromArgb(203, 213, 225),
            Font = AppTheme.CreateFont(15F, FontStyle.Bold),
            Padding = new Padding(14, 0, 6, 0),
            Margin = new Padding(0, 4, 0, 4),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => Navigate(text);
        _navButtons[text] = button;
        return button;
    }

    private Control BuildWorkspace()
    {
        var workspace = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        workspace.RowStyles.Add(new RowStyle(SizeType.Absolute, 88));
        workspace.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(AppTheme.SpaceXl, AppTheme.SpaceLg, AppTheme.SpaceXl, AppTheme.SpaceSm),
            ColumnCount = 2,
            RowCount = 1
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _pageTitle.Anchor = AnchorStyles.Left;
        _pageTitle.Margin = Padding.Empty;
        header.Controls.Add(_pageTitle, 0, 0);

        workspace.Controls.Add(header, 0, 0);
        workspace.Controls.Add(_content, 0, 1);
        return workspace;
    }

    private void Navigate(string page)
    {
        _currentPage = page;
        _marketTable = null;
        _watchlistControl = null;
        _watchlistSelectedStock = null;
        _watchlistSummary = null;
        _detailPrice = null;
        _detailChange = null;
        _detailSummary = null;
        _detailChart = null;
        _detailCandles.Clear();
        _pageTitle.Text = page;
        foreach (var pair in _navButtons)
        {
            var active = pair.Key == page;
            pair.Value.BackColor = active ? Color.FromArgb(30, 64, 175) : AppTheme.Sidebar;
            pair.Value.ForeColor = active ? Color.White : Color.FromArgb(203, 213, 225);
        }

        _content.Controls.Clear();
        Control view = page switch
        {
            "Tổng quan" => BuildAdminDashboard(),
            "Người dùng" => BuildUsersPage(),
            "Cổ phiếu" => BuildAdminStocksPage(),
            "Mô phỏng" => BuildSimulationPage(),
            "Thị trường" => BuildMarketPage(),
            "Chi tiết stock" => BuildStockDetailPage(_selectedStock),
            "Watchlist" => BuildWatchlistPage(),
            "Hồ sơ" => BuildProfilePage(),
            _ => new Panel()
        };
        view.Dock = DockStyle.Fill;
        _content.Controls.Add(view);
    }

    private async Task RefreshStocksAsync()
    {
        if (Interlocked.Exchange(ref _refreshStocksInFlight, 1) == 1)
        {
            return;
        }

        try
        {
            var serverData = await _stockService.GetAllAsync();
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(() =>
                {
                    if (!IsDisposed)
                    {
                        ApplyStocks(serverData);
                    }
                });
                return;
            }

            ApplyStocks(serverData);
        }
        catch (ObjectDisposedException)
        {
        }
        catch (InvalidOperationException) when (IsDisposed || !IsHandleCreated)
        {
        }
        catch
        {
            if (_stocks.Count == 0)
            {
                ApplyStockRows(MockData.Stocks);
            }
        }
        finally
        {
            Interlocked.Exchange(ref _refreshStocksInFlight, 0);
        }
    }

    private void ApplyStocks(IEnumerable<Stock> serverData)
    {
        foreach (var stock in serverData)
        {
            if (!_stockById.TryGetValue(stock.Id, out var row))
            {
                row = ToStockRow(stock);
                _stockById[row.Id] = row;
                _stocks.Add(row);
            }
            else
            {
                ApplyStock(row, stock);
            }
        }

        if (_stocks.Count > 0 && !_stockById.ContainsKey(_selectedStock.Id))
        {
            _selectedStock = _stocks[0];
        }

        RebindMarket();
    }

    private void ApplyStockRows(IEnumerable<StockRow> rows)
    {
        foreach (var source in rows)
        {
            if (!_stockById.TryGetValue(source.Id, out var row))
            {
                row = new StockRow();
                _stockById[source.Id] = row;
                _stocks.Add(row);
            }

            row.Id = source.Id;
            row.Symbol = source.Symbol;
            row.Company = source.Company;
            row.Sector = source.Sector;
            row.Price = source.Price;
            row.ChangePercent = source.ChangePercent;
            row.Volume = source.Volume;
            row.Active = source.Active;
        }

        if (_stocks.Count > 0 && !_stockById.ContainsKey(_selectedStock.Id))
        {
            _selectedStock = _stocks[0];
        }

        RebindMarket();
    }

    private void HandleStockPriceUpdated(object? sender, StockPriceUpdateDto update)
    {
        if (IsDisposed)
        {
            return;
        }

        lock (_pendingPriceUpdateLock)
        {
            _pendingPriceUpdates[update.StockId] = update;
        }

        if (!IsHandleCreated)
        {
            return;
        }

        try
        {
            BeginInvoke(() =>
            {
                if (IsDisposed)
                {
                    return;
                }

                if (!_priceUpdateTimer.Enabled)
                {
                    _priceUpdateTimer.Start();
                }
            });
        }
        catch (ObjectDisposedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
    }

    private void FlushPendingPriceUpdates()
    {
        if (IsDisposed)
        {
            return;
        }

        _priceUpdateTimer.Stop();
        List<StockPriceUpdateDto> updates;
        lock (_pendingPriceUpdateLock)
        {
            updates = _pendingPriceUpdates.Values.ToList();
            _pendingPriceUpdates.Clear();
        }

        var needsFullRefresh = false;
        foreach (var update in updates)
        {
            needsFullRefresh |= !ApplyStockPriceUpdate(update);
        }

        if (needsFullRefresh)
        {
            _ = RefreshStocksAsync();
        }
    }

    private bool ApplyStockPriceUpdate(StockPriceUpdateDto update)
    {
        if (!_stockById.TryGetValue(update.StockId, out var stock))
        {
            return false;
        }

        stock.Price = update.Price;
        stock.ChangePercent = update.ChangePercent;
        if (update.Volume > 0)
        {
            stock.Volume = update.Volume;
        }

        if (_selectedStock.Id == stock.Id)
        {
            _selectedStock = stock;
            _detailPrice?.Text = $"{stock.Price:N2}";
            if (_detailChange is not null)
            {
                _detailChange.Text = $"{stock.ChangePercent:+0.00;-0.00;0.00}%";
                _detailChange.ForeColor = stock.ChangePercent >= 0 ? AppTheme.Success : AppTheme.Danger;
            }
            _detailSummary?.SetStock(stock);
            UpdateDetailChart(update);
        }

        if (_watchlistSelectedStock?.Id == stock.Id)
        {
            _watchlistSummary?.SetStock(stock);
        }

        _watchlistControl?.RefreshStock(stock);
        return true;
    }

    private void UpdateDetailChart(StockPriceUpdateDto update)
    {
        if (_detailChart is null || _detailCandles.Count == 0)
        {
            return;
        }

        var time = AlignDetailTime(update.UpdatedAt.ToLocalTime());
        var previous = _detailCandles[^1];
        CandlePoint candle;

        if (previous.Time == time)
        {
            var close = update.Price;
            var tickVolume = CreateChartTickVolume(update);
            candle = new CandlePoint
            {
                Time = previous.Time,
                Open = previous.Open,
                High = Math.Max(Math.Max(previous.High, close), update.PreviousPrice),
                Low = Math.Min(Math.Min(previous.Low, close), update.PreviousPrice),
                Close = close,
                Volume = Math.Max(1, previous.Volume + tickVolume)
            };
            _detailCandles[^1] = candle;
        }
        else
        {
            var open = update.PreviousPrice > 0 ? update.PreviousPrice : previous.Close;
            var close = update.Price;
            var tickVolume = CreateChartTickVolume(update);
            candle = new CandlePoint
            {
                Time = time,
                Open = decimal.Round(open, 2),
                High = decimal.Round(Math.Max(open, close), 2),
                Low = decimal.Round(Math.Min(open, close), 2),
                Close = decimal.Round(close, 2),
                Volume = tickVolume
            };
            _detailCandles.Add(candle);
            while (_detailCandles.Count > _detailCandleMaxCount)
            {
                _detailCandles.RemoveAt(0);
            }
        }

        _detailChart.AddOrUpdateCandle(candle, _detailCandleMaxCount);
    }

    private static long CreateChartTickVolume(StockPriceUpdateDto update)
    {
        var priceBasis = Math.Max(update.Price, update.PreviousPrice);
        var movement = Math.Abs(update.Price - update.PreviousPrice);
        var activity = priceBasis <= 0 ? 1m : Math.Max(1m, movement / priceBasis * 10_000m);
        return Math.Max(1L, (long)Math.Round(activity * 100m, MidpointRounding.AwayFromZero));
    }

    private DateTime AlignDetailTime(DateTime time)
    {
        return _detailChartInterval switch
        {
            "1D" => time.Date,
            "1W" => AlignToWeek(time),
            "1M" => new DateTime(time.Year, time.Month, 1),
            _ => AlignToInterval(time, _detailCandleInterval)
        };
    }

    private static DateTime AlignToInterval(DateTime time, TimeSpan interval)
    {
        if (interval <= TimeSpan.Zero)
        {
            return time;
        }

        return new DateTime(time.Ticks - time.Ticks % interval.Ticks, time.Kind);
    }

    private static DateTime AlignToWeek(DateTime time)
    {
        var date = time.Date;
        var offset = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-offset);
    }

    private void RebindMarket()
    {
        if (_marketTable is null)
        {
            return;
        }

        var rows = _stocks
            .Where(stock => stock.Active && (string.IsNullOrWhiteSpace(_marketSearch)
                || stock.Symbol.Contains(_marketSearch, StringComparison.OrdinalIgnoreCase)
                || stock.Company.Contains(_marketSearch, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        _marketRows.RaiseListChangedEvents = false;
        _marketRows.Clear();
        foreach (var row in rows)
        {
            _marketRows.Add(row);
        }
        _marketRows.RaiseListChangedEvents = true;
        _marketRows.ResetBindings();
        _marketTable.SetData(_marketRows);
    }

}
