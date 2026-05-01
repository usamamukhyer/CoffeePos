namespace DBCafeteria.Domain.Entities;

public sealed class TipoGrano
{
    public int Id { get; set; }
    public string? Clave { get; set; }
    public string? Descripcion { get; set; }
    public byte[]? Imagen { get; set; }
    public double Precio { get; set; }
    public double TasaIva { get; set; }
    public double PrecioConIva { get; set; }
    public int? ClaveNumerica { get; set; }
    public double Existenciakilos { get; set; }
}
