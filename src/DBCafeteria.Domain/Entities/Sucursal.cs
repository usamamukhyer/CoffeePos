namespace DBCafeteria.Domain.Entities;

public sealed class Sucursal
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
