namespace DBCafeteria.Domain.Entities;

public sealed class SubClasificacionTopping
{
    public int Id { get; set; }
    public int IdSubClasificacion { get; set; }
    public int IdTopping { get; set; }
    public SubClasificacion? SubClasificacion { get; set; }
    public Topping? Topping { get; set; }
}
