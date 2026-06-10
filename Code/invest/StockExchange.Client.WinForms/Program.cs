using StockExchange.Client.WinForms.Forms;

namespace StockExchange.Client.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new LoginForm());
    }
}
