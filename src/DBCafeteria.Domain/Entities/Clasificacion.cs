namespace DBCafeteria.Domain.Entities;

public sealed class Clasificacion
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public byte[]? Imagen { get; set; }
    public string Clave { get; set; } = string.Empty;
    public int? ClaveNumerica { get; set; }
    public double? Precio { get; set; }
    public double? TasaIVA { get; set; }
    public double? PrecioConIVA { get; set; }
    public ICollection<SubClasificacion> SubClasificaciones { get; set; } = new List<SubClasificacion>();
}
