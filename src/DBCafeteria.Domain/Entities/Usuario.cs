namespace DBCafeteria.Domain.Entities;

public sealed class Usuario
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public byte[] Password { get; set; } = [];
    public string Nombre { get; set; } = string.Empty;
    public bool Inactivo { get; set; }
    public DateTime? FechaAlta { get; set; }
    public int Tipo { get; set; }
}
