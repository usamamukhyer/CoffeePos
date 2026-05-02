using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DBCafeteria.Models;

public sealed class BasketItemModel : INotifyPropertyChanged
{
    private int _quantity = 1;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public int? CupId { get; set; }
    public int? MilkId { get; set; }
    public int? BeanTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Temperature { get; set; } = "Caliente";
    public string Size { get; set; } = string.Empty;
    public string Milk { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal LineTotal => Price * Quantity;
    public string DetailsText
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(Temperature))
                parts.Add(Temperature);
            if (!string.IsNullOrWhiteSpace(Size))
                parts.Add(Size);
            if (!string.IsNullOrWhiteSpace(Milk))
                parts.Add(Milk);

            var toppingSummary = string.Join(", ", Toppings
                .Where(x => x.Quantity > 0)
                .Select(x => $"{x.Name} x{x.Quantity}"));
            if (!string.IsNullOrWhiteSpace(toppingSummary))
                parts.Add(toppingSummary);

            return string.Join("  |  ", parts);
        }
    }
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value)
                return;
            _quantity = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineTotal)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PriceText)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnitPriceText)));
        }
    }
    public string Image { get; set; } = "coffee_cup.png";
    public string PriceText => Services.AppCurrencySettings.Format(LineTotal);
    public string UnitPriceText => $"@ {Services.AppCurrencySettings.Format(Price)}";
    public List<BasketToppingModel> Toppings { get; set; } = [];
}

public sealed class BasketToppingModel
{
    public int ToppingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public double TaxRate { get; set; }
}
