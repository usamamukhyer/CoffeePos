namespace DBCafeteria.Domain.Entities;

public sealed class Topping
{
    public int Id { get; set; }
    public string? Clave { get; set; }
    public int? ClaveNumerica { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public byte[]? Imagen { get; set; }
    public int CantidadCortesia { get; set; }
    public double Precio { get; set; }
    public double TasaIVA { get; set; }
    public double PrecioConIVA { get; set; }
    public double Existencia { get; set; }
    public bool ShotExpresso { get; set; }
}
