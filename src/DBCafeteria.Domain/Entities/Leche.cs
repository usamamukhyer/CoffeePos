namespace DBCafeteria.Domain.Entities;

public sealed class Leche
{
    public int Id { get; set; }
    public string? Clave { get; set; }
    public string? Descripcion { get; set; }
    public int? ClaveNumerica { get; set; }
    public byte[]? Imagen { get; set; }
    public double Precio { get; set; }
    public double TasaIVA { get; set; }
    public double PrecioConIVA { get; set; }
    public double ExistenciaLitros { get; set; }
}
