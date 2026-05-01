namespace DBCafeteria.Models;

public sealed class CategoryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}
