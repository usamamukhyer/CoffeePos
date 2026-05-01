namespace DBCafeteria.Domain.Entities;

public sealed class SubClasificacion
{
    public int Id { get; set; }
    public string? Clave { get; set; }
    public int? ClaveNumerica { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public byte[]? Imagen { get; set; }
    public bool BebidasFrias { get; set; }
    public bool BebidasCalientes { get; set; }
    public double? Precio { get; set; }
    public double TasaIVA { get; set; }
    public double? PrecioConIVA { get; set; }
    public int IdClasificacion { get; set; }
    public Clasificacion? Clasificacion { get; set; }
}
