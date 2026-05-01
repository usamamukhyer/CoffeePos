namespace DBCafeteria.Domain.Entities;

public sealed class SubClasificacionTipoGrano
{
    public int Id { get; set; }
    public int IdSubClasificacion { get; set; }
    public int IdTipoGrano { get; set; }
    public SubClasificacion? SubClasificacion { get; set; }
    public TipoGrano? TipoGrano { get; set; }
}
