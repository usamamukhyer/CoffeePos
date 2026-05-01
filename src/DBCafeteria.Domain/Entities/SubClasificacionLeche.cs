namespace DBCafeteria.Domain.Entities;

public sealed class SubClasificacionLeche
{
    public int Id { get; set; }
    public int IdSubClasificacion { get; set; }
    public int IdLeche { get; set; }
    public SubClasificacion? SubClasificacion { get; set; }
    public Leche? Leche { get; set; }
}
