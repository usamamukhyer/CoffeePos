using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class ChoiceViewModel(string title, string detail = "", decimal price = 0, int id = 0) : BaseViewModel
{
    private bool _isSelected;
    private int _quantity;

    public int Id { get; } = id;
    public string Title { get; } = title;
    public string Detail { get; } = detail;
    public decimal Price { get; } = price;
    public string PriceText => AppCurrencySettings.Format(Price, showPlus: true);
    public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
    public int Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }
}
