namespace DBCafeteria.Domain.Entities;

public sealed class PedidoProductoTopping
{
    public int Id { get; set; }
    public int IdPedidoProducto { get; set; }
    public int IdTopping { get; set; }
    public int Cantidad { get; set; }
    public double Subtotal { get; set; }
    public double TasaIVA { get; set; }
    public double IVA { get; set; }
    public double Total { get; set; }
    public double Precio { get; set; }
    public PedidoProducto? PedidoProducto { get; set; }
}
