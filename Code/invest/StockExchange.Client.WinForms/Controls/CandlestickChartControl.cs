using System.Drawing.Drawing2D;
using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Mock;

namespace StockExchange.Client.WinForms.Controls;

public sealed class CandlestickChartControl : Control
{
    private const int PriceAxisWidth = 72;
    private const int TimeAxisHeight = 28;
    private const int VolumeHeight = 72;
    private IReadOnlyList<CandlePoint> _candles = Array.Empty<CandlePoint>();
    private int _hoverIndex = -1;
    private int _viewStart;
    private int _visibleCount;
    private int _defaultVisibleCount;
    private bool _isPanning;
    private int _panStartX;
    private int _panStartIndex;

    public CandlestickChartControl()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = AppTheme.Surface;
        Font = AppTheme.SmallFont;
        MinimumSize = new Size(480, 280);
        Margin = Padding.Empty;
        Cursor = Cursors.Cross;
    }

    public void SetCandles(IReadOnlyList<CandlePoint> candles, int defaultVisibleCount = 80)
    {
        _candles = candles;
        _defaultVisibleCount = Math.Clamp(defaultVisibleCount, 1, Math.Max(1, candles.Count));
        _hoverIndex = -1;
        ResetView();
    }

    public void ZoomIn() => ZoomBy(-Math.Max(2, _visibleCount / 5), GetZoomAnchor());

    public void ZoomOut() => ZoomBy(Math.Max(2, _visibleCount / 4), GetZoomAnchor());

    public void ResetView()
    {
        _visibleCount = Math.Min(_defaultVisibleCount, _candles.Count);
        _viewStart = Math.Max(0, _candles.Count - _visibleCount);
        Invalidate();
    }

    protected override void OnMouseWheel(MouseEventArgs eventArgs)
    {
        base.OnMouseWheel(eventArgs);
        if (_candles.Count < 2)
        {
            return;
        }

        var anchor = GetIndexAtX(eventArgs.X);
        var delta = eventArgs.Delta > 0
            ? -Math.Max(2, _visibleCount / 6)
            : Math.Max(2, _visibleCount / 5);
        ZoomBy(delta, anchor);
    }

    protected override void OnMouseDown(MouseEventArgs eventArgs)
    {
        base.OnMouseDown(eventArgs);
        if (eventArgs.Button != MouseButtons.Left || _visibleCount >= _candles.Count)
        {
            return;
        }

        _isPanning = true;
        _panStartX = eventArgs.X;
        _panStartIndex = _viewStart;
        Cursor = Cursors.SizeWE;
        Capture = true;
    }

    protected override void OnMouseUp(MouseEventArgs eventArgs)
    {
        base.OnMouseUp(eventArgs);
        _isPanning = false;
        Capture = false;
        Cursor = Cursors.Cross;
    }

    protected override void OnMouseMove(MouseEventArgs eventArgs)
    {
        base.OnMouseMove(eventArgs);
        var plot = GetPriceBounds();
        if (_isPanning)
        {
            var slot = plot.Width / (float)Math.Max(1, _visibleCount);
            var shift = (int)Math.Round((_panStartX - eventArgs.X) / Math.Max(1F, slot));
            _viewStart = Math.Clamp(_panStartIndex + shift, 0, Math.Max(0, _candles.Count - _visibleCount));
            Invalidate();
            return;
        }

        if (_candles.Count == 0 || !plot.Contains(eventArgs.Location))
        {
            _hoverIndex = -1;
        }
        else
        {
            _hoverIndex = GetIndexAtX(eventArgs.X);
        }
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs eventArgs)
    {
        base.OnMouseLeave(eventArgs);
        _hoverIndex = -1;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs eventArgs)
    {
        base.OnPaint(eventArgs);
        eventArgs.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        eventArgs.Graphics.Clear(BackColor);

        var priceBounds = GetPriceBounds();
        var volumeBounds = GetVolumeBounds();
        if (_candles.Count == 0 || priceBounds.Width <= 0 || priceBounds.Height <= 0)
        {
            TextRenderer.DrawText(eventArgs.Graphics, "Chưa có dữ liệu nến", AppTheme.BodyFont, ClientRectangle,
                AppTheme.Muted, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            return;
        }

        var visible = GetVisibleCandles();
        var minimum = visible.Min(candle => candle.Low);
        var maximum = visible.Max(candle => candle.High);
        var priceRange = Math.Max(0.01m, maximum - minimum);
        var maxVolume = Math.Max(1L, visible.Max(candle => candle.Volume));

        DrawGrid(eventArgs.Graphics, priceBounds, minimum, maximum);
        DrawTimeAxis(eventArgs.Graphics, priceBounds, visible);
        DrawCandles(eventArgs.Graphics, priceBounds, volumeBounds, visible, minimum, priceRange, maxVolume);
        DrawLastPrice(eventArgs.Graphics, priceBounds, minimum, priceRange);

        if (_hoverIndex >= 0)
        {
            DrawHover(eventArgs.Graphics, priceBounds, volumeBounds, minimum, priceRange);
        }
    }

    private Rectangle GetPriceBounds() => new(
        AppTheme.SpaceMd,
        AppTheme.SpaceMd,
        Math.Max(1, ClientSize.Width - PriceAxisWidth - AppTheme.SpaceMd * 2),
        Math.Max(1, ClientSize.Height - VolumeHeight - TimeAxisHeight - AppTheme.SpaceXl));

    private Rectangle GetVolumeBounds()
    {
        var price = GetPriceBounds();
        return new Rectangle(price.Left, price.Bottom + AppTheme.SpaceSm, price.Width, VolumeHeight);
    }

    private static void DrawGrid(Graphics graphics, Rectangle bounds, decimal minimum, decimal maximum)
    {
        using var gridPen = new Pen(AppTheme.Border);
        for (var index = 0; index <= 4; index++)
        {
            var y = bounds.Top + bounds.Height * index / 4;
            graphics.DrawLine(gridPen, bounds.Left, y, bounds.Right, y);
            var value = maximum - (maximum - minimum) * index / 4m;
            TextRenderer.DrawText(graphics, $"{value:N2}", AppTheme.SmallFont,
                new Rectangle(bounds.Right + 8, y - 10, PriceAxisWidth - 8, 20),
                AppTheme.Muted, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }
    }

    private void DrawTimeAxis(Graphics graphics, Rectangle bounds, IReadOnlyList<CandlePoint> visible)
    {
        var labels = Math.Min(6, visible.Count);
        for (var index = 0; index < labels; index++)
        {
            var candleIndex = labels == 1 ? 0 : index * (visible.Count - 1) / (labels - 1);
            var x = bounds.Left + candleIndex * bounds.Width / Math.Max(1, visible.Count - 1);
            var text = visible[candleIndex].Time.ToString(visible.Count > 40 ? "dd/MM" : "dd/MM HH:mm");
            TextRenderer.DrawText(graphics, text, AppTheme.SmallFont,
                new Rectangle(x - 40, bounds.Bottom + VolumeHeight + 10, 80, TimeAxisHeight),
                AppTheme.Muted, TextFormatFlags.HorizontalCenter);
        }
    }

    private void DrawCandles(
        Graphics graphics,
        Rectangle priceBounds,
        Rectangle volumeBounds,
        IReadOnlyList<CandlePoint> visible,
        decimal minimum,
        decimal priceRange,
        long maxVolume)
    {
        var slot = priceBounds.Width / (float)visible.Count;
        var gap = Math.Clamp(slot * 0.18F, 2F, 4F);
        var bodyWidth = Math.Max(3F, Math.Min(18F, slot - gap));

        for (var index = 0; index < visible.Count; index++)
        {
            var candle = visible[index];
            var rising = candle.Close >= candle.Open;
            var color = rising ? AppTheme.Success : AppTheme.Danger;
            var centerX = priceBounds.Left + slot * index + slot / 2F;
            var highY = PriceToY(candle.High, priceBounds, minimum, priceRange);
            var lowY = PriceToY(candle.Low, priceBounds, minimum, priceRange);
            var openY = PriceToY(candle.Open, priceBounds, minimum, priceRange);
            var closeY = PriceToY(candle.Close, priceBounds, minimum, priceRange);

            using var pen = new Pen(color, 1.2F);
            using var brush = new SolidBrush(color);
            graphics.DrawLine(pen, centerX, highY, centerX, lowY);
            var bodyTop = Math.Min(openY, closeY);
            var bodyHeight = Math.Max(2F, Math.Abs(openY - closeY));
            graphics.FillRectangle(brush, centerX - bodyWidth / 2F, bodyTop, bodyWidth, bodyHeight);

            var volumeHeight = Math.Max(1F, candle.Volume / (float)maxVolume * volumeBounds.Height);
            using var volumeBrush = new SolidBrush(Color.FromArgb(90, color));
            graphics.FillRectangle(volumeBrush, centerX - bodyWidth / 2F,
                volumeBounds.Bottom - volumeHeight, bodyWidth, volumeHeight);
        }
    }

    private void DrawLastPrice(Graphics graphics, Rectangle bounds, decimal minimum, decimal priceRange)
    {
        var last = _candles[Math.Min(_candles.Count - 1, _viewStart + _visibleCount - 1)];
        var y = PriceToY(last.Close, bounds, minimum, priceRange);
        var color = last.Close >= last.Open ? AppTheme.Success : AppTheme.Danger;
        using var pen = new Pen(color) { DashStyle = DashStyle.Dash };
        graphics.DrawLine(pen, bounds.Left, y, bounds.Right, y);
        using var brush = new SolidBrush(color);
        graphics.FillRectangle(brush, bounds.Right, y - 10, PriceAxisWidth - 4, 20);
        TextRenderer.DrawText(graphics, $"{last.Close:N2}", AppTheme.SmallFont,
            new Rectangle(bounds.Right + 4, (int)Math.Round(y) - 10, PriceAxisWidth - 8, 20),
            Color.White, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
    }

    private void DrawHover(
        Graphics graphics,
        Rectangle priceBounds,
        Rectangle volumeBounds,
        decimal minimum,
        decimal priceRange)
    {
        if (_hoverIndex < _viewStart || _hoverIndex >= _viewStart + _visibleCount)
        {
            return;
        }

        var candle = _candles[_hoverIndex];
        var slot = priceBounds.Width / (float)_visibleCount;
        var x = priceBounds.Left + slot * (_hoverIndex - _viewStart) + slot / 2F;
        var y = PriceToY(candle.Close, priceBounds, minimum, priceRange);
        using var pen = new Pen(Color.FromArgb(130, AppTheme.Muted)) { DashStyle = DashStyle.Dot };
        graphics.DrawLine(pen, x, priceBounds.Top, x, volumeBounds.Bottom);
        graphics.DrawLine(pen, priceBounds.Left, y, priceBounds.Right, y);

        var text = $"{candle.Time:dd/MM HH:mm}   O {candle.Open:N2}   H {candle.High:N2}   L {candle.Low:N2}   C {candle.Close:N2}   Vol {candle.Volume:N0}";
        var textSize = TextRenderer.MeasureText(text, AppTheme.SmallFont);
        var tooltip = new Rectangle(
            Math.Min(priceBounds.Right - textSize.Width - 16, Math.Max(priceBounds.Left, (int)x + 12)),
            priceBounds.Top + 8,
            textSize.Width + 16,
            textSize.Height + 10);
        using var background = new SolidBrush(Color.FromArgb(235, AppTheme.Sidebar));
        graphics.FillRectangle(background, tooltip);
        TextRenderer.DrawText(graphics, text, AppTheme.SmallFont, tooltip, Color.White,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    private static float PriceToY(decimal price, Rectangle bounds, decimal minimum, decimal range) =>
        bounds.Bottom - (float)((price - minimum) / range) * bounds.Height;

    private IReadOnlyList<CandlePoint> GetVisibleCandles()
    {
        var count = Math.Clamp(_visibleCount, 1, _candles.Count);
        var start = Math.Clamp(_viewStart, 0, _candles.Count - count);
        return _candles.Skip(start).Take(count).ToArray();
    }

    private int GetIndexAtX(int x)
    {
        var plot = GetPriceBounds();
        var slot = plot.Width / (float)Math.Max(1, _visibleCount);
        var localIndex = Math.Clamp((int)((x - plot.Left) / Math.Max(1F, slot)), 0, _visibleCount - 1);
        return Math.Clamp(_viewStart + localIndex, 0, _candles.Count - 1);
    }

    private int GetZoomAnchor() =>
        _hoverIndex >= 0 ? _hoverIndex : _viewStart + Math.Max(0, _visibleCount - 1) / 2;

    private void ZoomBy(int countDelta, int anchorIndex)
    {
        if (_candles.Count < 2)
        {
            return;
        }

        var oldCount = Math.Max(1, _visibleCount);
        var newCount = Math.Clamp(oldCount + countDelta, Math.Min(8, _candles.Count), _candles.Count);
        if (newCount == oldCount)
        {
            return;
        }

        var anchorRatio = (anchorIndex - _viewStart) / (float)oldCount;
        _viewStart = (int)Math.Round(anchorIndex - anchorRatio * newCount);
        _visibleCount = newCount;
        _viewStart = Math.Clamp(_viewStart, 0, Math.Max(0, _candles.Count - _visibleCount));
        Invalidate();
    }
}
