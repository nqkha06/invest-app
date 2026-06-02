namespace StockExchange.Client.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        using var mainForm = new Form
        {
            Text = "Stock Exchange Client",
            StartPosition = FormStartPosition.CenterScreen,
            Width = 900,
            Height = 600
        };

        var titleLabel = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            Text = "Stock Exchange Client is running.",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 16F, FontStyle.Bold)
        };

        mainForm.Controls.Add(titleLabel);
        Application.Run(mainForm);
    }
}
