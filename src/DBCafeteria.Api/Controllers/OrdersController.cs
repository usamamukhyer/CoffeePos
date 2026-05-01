using DBCafeteria.Application.DTOs;
using DBCafeteria.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DBCafeteria.Api.Controllers;

[ApiController]
[Route("orders")]
[Route("api/orders")]
public sealed class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<CreateOrderResponse>> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await orderService.CreateOrderAsync(request, cancellationToken)); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("place-order")]
    public async Task<ActionResult<CreateOrderResponse>> PlaceOrder(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await orderService.CreateOrderAsync(request, cancellationToken)); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
