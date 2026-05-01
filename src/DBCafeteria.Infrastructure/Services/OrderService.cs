using DBCafeteria.Application.DTOs;
using DBCafeteria.Application.Interfaces;
using DBCafeteria.Domain.Entities;
using DBCafeteria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DBCafeteria.Infrastructure.Services;

public sealed class OrderService(CafeDbContext db, IGiftService giftService) : IOrderService
{
    public CartValidationResponse ValidateCart(CartValidationRequest request)
    {
        var messages = new List<string>();
        if (request.Items.Count == 0)
            messages.Add("Cart is empty.");

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
                messages.Add($"Product {item.ProductId} quantity must be greater than zero.");
            if (item.UnitPrice < 0)
                messages.Add($"Product {item.ProductId} price cannot be negative.");
        }

        var subtotal = request.Items.Sum(x => x.UnitPrice * x.Quantity + x.Toppings.Sum(t => t.UnitPrice * t.Quantity));
        var tax = request.Items.Sum(x => x.UnitPrice * x.Quantity * x.TaxRate + x.Toppings.Sum(t => t.UnitPrice * t.Quantity * t.TaxRate));
        return new CartValidationResponse(messages.Count == 0, subtotal, tax, subtotal + tax, messages);
    }

    public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        if (request.BranchId <= 0)
            throw new InvalidOperationException("BranchId is required.");

        var branchExists = await db.Sucursales
            .AnyAsync(branch => branch.Id == request.BranchId && branch.Activo, cancellationToken);
        if (!branchExists)
            throw new InvalidOperationException("Selected branch does not exist or is inactive.");

        if (request.OrderType.Equals("Gift", StringComparison.OrdinalIgnoreCase))
        {
            if (request.Gift is null)
                throw new InvalidOperationException("Gift details are required for gift orders.");

            var recipientExists = await db.Clientes
                .AnyAsync(customer => customer.Id == request.Gift.RecipientCustomerId && customer.Activo, cancellationToken);
            if (!recipientExists)
                throw new InvalidOperationException("Please select an existing customer.");
        }

        var pickupType = string.IsNullOrWhiteSpace(request.PickupType) ? "Now" : request.PickupType.Trim();
        if (pickupType.Equals("PreOrder", StringComparison.OrdinalIgnoreCase))
        {
            if (request.PickupDateTime is null)
                throw new InvalidOperationException("PickupDateTime is required for preorder.");
        }
        else if (pickupType.Equals("Now", StringComparison.OrdinalIgnoreCase))
        {
            pickupType = "Now";
        }
        else
        {
            throw new InvalidOperationException("PickupType must be Now or PreOrder.");
        }

        var pricedItems = await BuildPricedItemsAsync(request.Items, cancellationToken);
        var validation = ValidateCart(new CartValidationRequest(pricedItems));
        if (!validation.IsValid)
            throw new InvalidOperationException(string.Join(" ", validation.Messages));

        var nextOrderNumber = await db.Pedidos.AnyAsync(cancellationToken)
            ? await db.Pedidos.MaxAsync(x => x.NoPedido, cancellationToken) + 1
            : 1;

        var pedido = new Pedido
        {
            Fecha = DateTime.Now,
            Subtotal = validation.Subtotal,
            IVA = validation.Tax,
            Total = validation.Total,
            IdCliente = request.CustomerId,
            Estado = 0,
            NoPedido = nextOrderNumber,
            Cambio = 0,
            TipoPedido = request.OrderType,
            TipoPickup = pickupType.Equals("PreOrder", StringComparison.OrdinalIgnoreCase) ? "PreOrder" : "Now",
            FechaPickup = pickupType.Equals("PreOrder", StringComparison.OrdinalIgnoreCase) ? request.PickupDateTime : null,
            IdSucursal = request.BranchId
        };

        foreach (var item in pricedItems)
        {
            var itemSubtotal = item.UnitPrice * item.Quantity;
            var itemTax = itemSubtotal * item.TaxRate;
            var pedidoProducto = new PedidoProducto
            {
                IdClasificacion = item.CategoryId,
                IdSubClasificacion = item.ProductId,
                TipoBebida = item.DrinkType,
                IdVaso = item.CupId,
                IdLeche = item.MilkId,
                IdTipoGrano = item.BeanTypeId,
                Cantidad = item.Quantity,
                Precio = item.UnitPrice,
                TasaIVA = item.TaxRate,
                Subtotal = itemSubtotal,
                IVA = itemTax,
                Total = itemSubtotal + itemTax
            };

            foreach (var topping in item.Toppings)
            {
                var toppingSubtotal = topping.UnitPrice * topping.Quantity;
                var toppingTax = toppingSubtotal * topping.TaxRate;
                pedidoProducto.Toppings.Add(new PedidoProductoTopping
                {
                    IdTopping = topping.ToppingId,
                    Cantidad = topping.Quantity,
                    Precio = topping.UnitPrice,
                    TasaIVA = topping.TaxRate,
                    Subtotal = toppingSubtotal,
                    IVA = toppingTax,
                    Total = toppingSubtotal + toppingTax
                });
            }

            pedido.Productos.Add(pedidoProducto);
        }

        var payment = request.Payment ?? new PaymentRequest("Cash on Delivery", "MXN", validation.Total, 1, 0);
        var exchangeRate = payment.ExchangeRate <= 0 ? 1 : payment.ExchangeRate;
        var paymentAmount = validation.Total;
        pedido.Pagos.Add(new PedidoPago
        {
            FormaPago = string.IsNullOrWhiteSpace(payment.PaymentMethod) ? "Cash on Delivery" : payment.PaymentMethod.Trim(),
            Moneda = string.IsNullOrWhiteSpace(payment.Currency) ? "MXN" : payment.Currency.Trim(),
            TipoCambio = exchangeRate,
            Pago = paymentAmount,
            PagoMXN = paymentAmount * exchangeRate,
            IdUsuario = await ResolvePaymentUserIdAsync(payment.UserId, cancellationToken),
            Estado = "Pending"
        });

        db.Pedidos.Add(pedido);
        await db.SaveChangesAsync(cancellationToken);

        string? giftCode = null;
        if (request.OrderType.Equals("Gift", StringComparison.OrdinalIgnoreCase) && request.Gift is not null)
        {
            var gift = await giftService.CreateGiftAsync(pedido.Id, request.Gift, request.CustomerId, cancellationToken);
            giftCode = gift.GiftCode;
        }

        return new CreateOrderResponse(pedido.Id, pedido.NoPedido, pedido.Total, giftCode);
    }

    private async Task<int> ResolvePaymentUserIdAsync(int requestedUserId, CancellationToken cancellationToken)
    {
        if (requestedUserId > 0)
        {
            var exists = await db.Usuarios.AnyAsync(user => user.Id == requestedUserId && !user.Inactivo, cancellationToken);
            if (exists)
                return requestedUserId;
        }

        const string systemUserName = "mobile-cod";
        var systemUser = await db.Usuarios.FirstOrDefaultAsync(user => user.UserName == systemUserName, cancellationToken);
        if (systemUser is not null)
            return systemUser.Id;

        systemUser = new Usuario
        {
            UserName = systemUserName,
            Password = [],
            Nombre = "Mobile Cash on Delivery",
            Inactivo = false,
            FechaAlta = DateTime.Now,
            Tipo = 0
        };
        db.Usuarios.Add(systemUser);
        await db.SaveChangesAsync(cancellationToken);
        return systemUser.Id;
    }

    private async Task<IReadOnlyList<CreateOrderItemRequest>> BuildPricedItemsAsync(IReadOnlyList<CreateOrderItemRequest> items, CancellationToken cancellationToken)
    {
        if (items.Count == 0)
            return items;

        var pricedItems = new List<CreateOrderItemRequest>(items.Count);
        foreach (var item in items)
        {
            var product = await db.SubClasificaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == item.ProductId && x.IdClasificacion == item.CategoryId, cancellationToken);
            if (product is null)
                throw new InvalidOperationException($"Product {item.ProductId} was not found.");

            var unitPrice = product.PrecioConIVA ?? product.Precio ?? 0;

            if (item.CupId is int cupId)
            {
                var cup = await db.SubClasificacionesVasos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdSubClasificacion == item.ProductId && x.IdVaso == cupId, cancellationToken);
                if (cup is null)
                    throw new InvalidOperationException("Selected cup size is not available for this product.");

                unitPrice += cup.PrecioConIVA ?? cup.Precio ?? 0;
            }

            if (item.MilkId is int milkId)
            {
                var milk = await db.SubClasificacionesLeches
                    .AsNoTracking()
                    .Where(x => x.IdSubClasificacion == item.ProductId && x.IdLeche == milkId)
                    .Select(x => x.Leche)
                    .FirstOrDefaultAsync(cancellationToken);
                if (milk is null)
                    throw new InvalidOperationException("Selected milk is not available for this product.");

                unitPrice += milk.PrecioConIVA;
            }

            if (item.BeanTypeId is int beanTypeId)
            {
                var bean = await db.SubClasificacionesTiposGranos
                    .AsNoTracking()
                    .Where(x => x.IdSubClasificacion == item.ProductId && x.IdTipoGrano == beanTypeId)
                    .Select(x => x.TipoGrano)
                    .FirstOrDefaultAsync(cancellationToken);
                if (bean is null)
                    throw new InvalidOperationException("Selected bean type is not available for this product.");

                unitPrice += bean.PrecioConIva;
            }

            var pricedToppings = new List<CreateOrderToppingRequest>();
            foreach (var toppingRequest in item.Toppings.Where(x => x.Quantity > 0))
            {
                var topping = await db.SubClasificacionesToppings
                    .AsNoTracking()
                    .Where(x => x.IdSubClasificacion == item.ProductId && x.IdTopping == toppingRequest.ToppingId)
                    .Select(x => x.Topping)
                    .FirstOrDefaultAsync(cancellationToken);
                if (topping is null)
                    throw new InvalidOperationException("Selected topping is not available for this product.");

                pricedToppings.Add(new CreateOrderToppingRequest(topping.Id, toppingRequest.Quantity, topping.PrecioConIVA, 0));
            }

            pricedItems.Add(item with
            {
                UnitPrice = unitPrice,
                TaxRate = 0,
                Toppings = pricedToppings
            });
        }

        return pricedItems;
    }
}
