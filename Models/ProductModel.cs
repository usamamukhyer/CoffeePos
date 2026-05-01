namespace DBCafeteria.Models;

public sealed class ProductModel
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subtitle { get; set; } = "EXPLORE ITEMS";
    public string Image { get; set; } = "coffee_cup.png";
    public decimal BasePrice { get; set; }
    public bool HotAvailable { get; set; } = true;
    public bool ColdAvailable { get; set; } = true;
}
