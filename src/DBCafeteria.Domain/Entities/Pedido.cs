namespace DBCafeteria.Domain.Entities;

public sealed class Pedido
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public double Subtotal { get; set; }
    public double IVA { get; set; }
    public double Total { get; set; }
    public int? IdCliente { get; set; }
    public int Estado { get; set; }
    public int NoPedido { get; set; }
    public double Cambio { get; set; }
    public int? IdUsuarioPreparo { get; set; }
    public int? IdUsuarioEntrego { get; set; }
    public int? IdSucursal { get; set; }
    public string TipoPedido { get; set; } = "ForMe";
    public string TipoPickup { get; set; } = "Now";
    public DateTime? FechaPickup { get; set; }
    public string? Ubicacion { get; set; }
    public Cliente? Cliente { get; set; }
    public Sucursal? Sucursal { get; set; }
    public ICollection<PedidoProducto> Productos { get; set; } = new List<PedidoProducto>();
    public ICollection<PedidoPago> Pagos { get; set; } = new List<PedidoPago>();
    public Regalo? Regalo { get; set; }
}
