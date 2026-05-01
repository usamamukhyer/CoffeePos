namespace DBCafeteria.Models;

public sealed class CustomizationOptionModel
{
    public string Name { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsSelected { get; set; }
}
