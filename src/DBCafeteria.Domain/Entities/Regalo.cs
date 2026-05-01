namespace DBCafeteria.Domain.Entities;

public sealed class Regalo
{
    public int Id { get; set; }
    public int IdPedido { get; set; }
    public int? IdClienteRemitente { get; set; }
    public int? IdClienteReceptor { get; set; }
    public string NombreReceptor { get; set; } = string.Empty;
    public string TelefonoReceptor { get; set; } = string.Empty;
    public string? Mensaje { get; set; }
    public string? CodigoRegalo { get; set; }
    public string Estado { get; set; } = "Pending";
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public DateTime? FechaRedimido { get; set; }
    public Pedido? Pedido { get; set; }
}
