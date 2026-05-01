namespace DBCafeteria.Domain.Entities;

public sealed class Cliente
{
    public int Id { get; set; }
    public long? Telefono { get; set; }
    public string? Nombre { get; set; }
    public bool EsInvitado { get; set; }
    public string? Email { get; set; }
    public byte[]? PasswordHash { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
