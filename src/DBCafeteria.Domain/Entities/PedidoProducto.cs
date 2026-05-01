namespace DBCafeteria.Domain.Entities;

public sealed class PedidoProducto
{
    public int Id { get; set; }
    public int IdPedido { get; set; }
    public int IdClasificacion { get; set; }
    public int? TipoBebida { get; set; }
    public int? IdVaso { get; set; }
    public int? IdLeche { get; set; }
    public double Subtotal { get; set; }
    public double TasaIVA { get; set; }
    public double IVA { get; set; }
    public double Total { get; set; }
    public int Cantidad { get; set; }
    public int IdSubClasificacion { get; set; }
    public double Precio { get; set; }
    public double PrecioLeche { get; set; }
    public double TasaIVALeche { get; set; }
    public double PrecioConIVALeche { get; set; }
    public int? IdTipoGrano { get; set; }
    public double PrecioTipoGrano { get; set; }
    public double PrecioConIVATipoGrano { get; set; }
    public double TasaIVATipoGrano { get; set; }
    public double CantidadLeche { get; set; }
    public double CantidadCafe { get; set; }
    public Pedido? Pedido { get; set; }
    public ICollection<PedidoProductoTopping> Toppings { get; set; } = new List<PedidoProductoTopping>();
}
