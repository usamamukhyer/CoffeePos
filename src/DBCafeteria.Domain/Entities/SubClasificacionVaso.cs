namespace DBCafeteria.Domain.Entities;

public sealed class SubClasificacionVaso
{
    public int Id { get; set; }
    public int IdSubClasificacion { get; set; }
    public int IdVaso { get; set; }
    public double? Precio { get; set; }
    public double? PrecioConIVA { get; set; }
    public double CantidadLeche { get; set; }
    public double GramosCafe { get; set; }
    public SubClasificacion? SubClasificacion { get; set; }
    public Vaso? Vaso { get; set; }
}
