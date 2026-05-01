namespace DBCafeteria.Domain.Entities;

public sealed class PedidoPago
{
    public int Id { get; set; }
    public int IdPedido { get; set; }
    public string FormaPago { get; set; } = "Card";
    public string Moneda { get; set; } = "MXN";
    public double TipoCambio { get; set; } = 1;
    public double Pago { get; set; }
    public double PagoMXN { get; set; }
    public int IdUsuario { get; set; }
    public string Estado { get; set; } = "Pending";
    public Pedido? Pedido { get; set; }
}
