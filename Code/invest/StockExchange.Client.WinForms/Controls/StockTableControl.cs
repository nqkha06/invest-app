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
        
        _grid.AutoGenerateColumns = false;
        
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StockRow.Symbol), Name = nameof(StockRow.Symbol), HeaderText = "Mã" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StockRow.Company), Name = nameof(StockRow.Company), HeaderText = "Doanh nghiệp" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StockRow.Sector), Name = nameof(StockRow.Sector), HeaderText = "Ngành" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StockRow.Price), Name = nameof(StockRow.Price), HeaderText = "Giá" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StockRow.ChangePercent), Name = nameof(StockRow.ChangePercent), HeaderText = "% thay đổi" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StockRow.Volume), Name = nameof(StockRow.Volume), HeaderText = "Khối lượng" });

        SetColumn(nameof(StockRow.Symbol), 80, 12F);
        SetColumn(nameof(StockRow.Company), 180, 28F);
        SetColumn(nameof(StockRow.Sector), 120, 18F);
        SetColumn(nameof(StockRow.Price), 90, 14F);
        SetColumn(nameof(StockRow.ChangePercent), 110, 14F);
        SetColumn(nameof(StockRow.Volume), 110, 14F);

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
    }

    public StockRow? SelectedStock => _grid.CurrentRow?.DataBoundItem as StockRow;

    public void RefreshData()
    {
        _grid.Refresh();
    }

    private void RaiseSelected()
    {
        if (SelectedStock is { } stock)
        {
            StockSelected?.Invoke(this, stock);
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
