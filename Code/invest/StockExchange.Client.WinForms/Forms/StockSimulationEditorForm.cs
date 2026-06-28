using StockExchange.Client.WinForms.Helpers;
using StockExchange.Shared.DTOs;

namespace StockExchange.Client.WinForms.Forms;

public sealed class StockSimulationEditorForm : Form
{
    private readonly StockSimulationConfigDto _simulation;
    private readonly ComboBox _algorithm;
    private readonly NumericUpDown _volatility;
    private readonly NumericUpDown _trend;
    private readonly NumericUpDown _minPrice;
    private readonly NumericUpDown _maxPrice;
    private readonly NumericUpDown _speed;
    private readonly NumericUpDown _jump;

    public StockSimulationUpdateDto Result { get; private set; } = new();

    public StockSimulationEditorForm(StockSimulationConfigDto simulation)
    {
        _simulation = simulation;
        Text = $"Simulation - {simulation.Symbol}";
        Width = 460;
        Height = 620;
        MinimumSize = new Size(420, 560);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = AppTheme.Background;

        var content = AppTheme.CreateCard(24);
        content.Dock = DockStyle.Fill;
        content.Margin = Padding.Empty;
        content.Padding = new Padding(28);
        Controls.Add(content);

        var form = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };
        content.Controls.Add(form);

        form.Controls.Add(AppTheme.CreateLabel($"{simulation.Symbol} - {simulation.CompanyName}", 16F, FontStyle.Bold));
        form.Controls.Add(AppTheme.CreateLabel("Cấu hình thuật toán mô phỏng giá.", 9F, FontStyle.Regular, AppTheme.Muted));

        _algorithm = AddComboField(form, "Thuật toán", ["RandomWalk", "MeanReversion", "TrendFollowing"]);
        _volatility = AddNumericField(form, "Volatility", 4, simulation.Volatility);
        _trend = AddNumericField(form, "Trend factor", 4, simulation.TrendFactor);
        _minPrice = AddNumericField(form, "Giá tối thiểu", 2, simulation.MinPrice, 1_000_000);
        _maxPrice = AddNumericField(form, "Giá tối đa", 2, simulation.MaxPrice, 1_000_000);
        _speed = AddNumericField(form, "Tốc độ cập nhật (giây)", 2, simulation.UpdateSpeed, 60);
        _jump = AddNumericField(form, "Xác suất jump", 4, simulation.JumpProbability);

        _algorithm.SelectedItem = string.IsNullOrWhiteSpace(simulation.AlgorithmType)
            ? "RandomWalk"
            : simulation.AlgorithmType;

        var actions = new FlowLayoutPanel
        {
            Width = 360,
            Height = 56,
            FlowDirection = FlowDirection.RightToLeft,
            Margin = new Padding(0, 12, 0, 0)
        };
        var save = AppTheme.CreateButton("Lưu cấu hình");
        var cancel = AppTheme.CreateButton("Hủy", false);
        actions.Controls.Add(save);
        actions.Controls.Add(cancel);
        form.Controls.Add(actions);

        save.Click += (_, _) => Save();
        cancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };
    }

    private void Save()
    {
        if (_minPrice.Value >= _maxPrice.Value)
        {
            MessageBox.Show(this, "Giá tối thiểu phải nhỏ hơn giá tối đa.",
                "Cấu hình không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_speed.Value <= 0)
        {
            MessageBox.Show(this, "Tốc độ cập nhật phải lớn hơn 0.",
                "Cấu hình không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Result = new StockSimulationUpdateDto
        {
            Id = _simulation.Id,
            AlgorithmType = _algorithm.Text,
            Volatility = _volatility.Value,
            TrendFactor = _trend.Value,
            MinPrice = _minPrice.Value,
            MaxPrice = _maxPrice.Value,
            UpdateSpeed = _speed.Value,
            JumpProbability = _jump.Value
        };

        DialogResult = DialogResult.OK;
        Close();
    }

    private static ComboBox AddComboField(FlowLayoutPanel form, string label, string[] values)
    {
        form.Controls.Add(AppTheme.CreateLabel(label, 9.5F, FontStyle.Bold));
        var combo = new ComboBox
        {
            Width = 360,
            Height = 36,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = AppTheme.BodyFont,
            Margin = new Padding(3, 6, 3, 12)
        };
        combo.Items.AddRange(values);
        combo.SelectedIndex = 0;
        form.Controls.Add(combo);
        return combo;
    }

    private static NumericUpDown AddNumericField(
        FlowLayoutPanel form, string label, int decimals, decimal value, decimal maximum = 1)
    {
        form.Controls.Add(AppTheme.CreateLabel(label, 9.5F, FontStyle.Bold));
        var input = new NumericUpDown
        {
            Width = 360,
            Height = 36,
            DecimalPlaces = decimals,
            Increment = decimals >= 3 ? 0.001m : 0.01m,
            Minimum = 0,
            Maximum = maximum,
            Value = Math.Min(maximum, Math.Max(0, value)),
            Font = AppTheme.BodyFont,
            Margin = new Padding(3, 6, 3, 12)
        };
        form.Controls.Add(input);
        return input;
    }
}
