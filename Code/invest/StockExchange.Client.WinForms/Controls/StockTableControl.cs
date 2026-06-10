using StockExchange.Client.WinForms.Helpers;
using StockExchange.Client.WinForms.Mock;

namespace StockExchange.Client.WinForms.Controls;

public class StockTableControl : UserControl
{
    private readonly DataGridView _grid = AppTheme.CreateGrid();

    public event EventHandler<StockRow>? StockSelected;

    public StockTableControl()
    {
        Dock = DockStyle.Fill;
        BackColor = AppTheme.Surface;
        Margin = Padding.Empty;
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_grid);
        _grid.CellDoubleClick += (_, _) => RaiseSelected();
        _grid.KeyDown += (_, eventArgs) =>
        {
            if (eventArgs.KeyCode == Keys.Enter)
            {
                eventArgs.SuppressKeyPress = true;
                RaiseSelected();
            }
        };
    }

    public void SetData(object dataSource)
    {
        _grid.DataSource = dataSource;
        ConfigureColumns();
    }

    public StockRow? SelectedStock => _grid.CurrentRow?.DataBoundItem as StockRow;

    private void RaiseSelected()
    {
        if (SelectedStock is { } stock)
        {
            StockSelected?.Invoke(this, stock);
        }
    }

    private void ConfigureColumns()
    {
        foreach (DataGridViewColumn column in _grid.Columns)
        {
            column.Visible = column.Name is nameof(StockRow.Symbol)
                or nameof(StockRow.Company)
                or nameof(StockRow.Sector)
                or nameof(StockRow.Price)
                or nameof(StockRow.ChangePercent)
                or nameof(StockRow.Volume);
        }
        SetHeader(nameof(StockRow.Symbol), "Mã");
        SetHeader(nameof(StockRow.Company), "Doanh nghiệp");
        SetHeader(nameof(StockRow.Sector), "Ngành");
        SetHeader(nameof(StockRow.Price), "Giá");
        SetHeader(nameof(StockRow.ChangePercent), "% thay đổi");
        SetHeader(nameof(StockRow.Volume), "Khối lượng");

        SetColumn(nameof(StockRow.Symbol), 80, 12F);
        SetColumn(nameof(StockRow.Company), 180, 28F);
        SetColumn(nameof(StockRow.Sector), 120, 18F);
        SetColumn(nameof(StockRow.Price), 90, 14F);
        SetColumn(nameof(StockRow.ChangePercent), 110, 14F);
        SetColumn(nameof(StockRow.Volume), 110, 14F);
    }

    private void SetHeader(string name, string text)
    {
        if (_grid.Columns[name] is { } column)
        {
            column.HeaderText = text;
        }
    }

    private void SetColumn(string name, int minimumWidth, float fillWeight)
    {
        if (_grid.Columns[name] is not { } column)
        {
            return;
        }

        column.MinimumWidth = minimumWidth;
        column.FillWeight = fillWeight;
        column.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
    }
}
