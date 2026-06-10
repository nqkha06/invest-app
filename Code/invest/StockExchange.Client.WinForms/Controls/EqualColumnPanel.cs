using System.ComponentModel;

namespace StockExchange.Client.WinForms.Controls;

public sealed class EqualColumnPanel : Panel
{
    [DefaultValue(1)]
    public int Columns { get; set; } = 1;

    [DefaultValue(12)]
    public int Gap { get; set; } = 12;

    public EqualColumnPanel()
    {
        DoubleBuffered = true;
        Margin = System.Windows.Forms.Padding.Empty;
        Padding = System.Windows.Forms.Padding.Empty;
    }

    protected override void OnLayout(LayoutEventArgs eventArgs)
    {
        base.OnLayout(eventArgs);

        if (Columns <= 0 || Controls.Count == 0)
        {
            return;
        }

        var visibleControls = Controls.Cast<Control>().Where(control => control.Visible).ToArray();
        if (visibleControls.Length == 0)
        {
            return;
        }

        var columnCount = Math.Min(Columns, visibleControls.Length);
        var availableWidth = Math.Max(0, ClientSize.Width - Padding.Horizontal - Gap * (columnCount - 1));
        var baseWidth = availableWidth / columnCount;
        var remainder = availableWidth % columnCount;
        var left = Padding.Left;
        var height = Math.Max(0, ClientSize.Height - Padding.Vertical);

        for (var index = 0; index < visibleControls.Length; index++)
        {
            var column = index % columnCount;
            var width = baseWidth + (column < remainder ? 1 : 0);
            visibleControls[index].Bounds = new Rectangle(left, Padding.Top, width, height);
            visibleControls[index].Margin = System.Windows.Forms.Padding.Empty;
            left += width + Gap;
        }
    }
}
