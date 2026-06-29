using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StockExchange.Client.WinForms.Models;

public sealed class StockRow : INotifyPropertyChanged
{
    private long _id;
    private string _symbol = string.Empty;
    private string _company = string.Empty;
    private string _sector = string.Empty;
    private decimal _price;
    private decimal _changePercent;
    private long _volume;
    private bool _active;

    public event PropertyChangedEventHandler? PropertyChanged;

    public long Id { get => _id; set => SetField(ref _id, value); }
    public string Symbol { get => _symbol; set => SetField(ref _symbol, value); }
    public string Company { get => _company; set => SetField(ref _company, value); }
    public string Sector { get => _sector; set => SetField(ref _sector, value); }
    public decimal Price { get => _price; set => SetField(ref _price, value); }
    public decimal ChangePercent { get => _changePercent; set => SetField(ref _changePercent, value); }
    public long Volume { get => _volume; set => SetField(ref _volume, value); }
    public bool Active { get => _active; set => SetField(ref _active, value); }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
