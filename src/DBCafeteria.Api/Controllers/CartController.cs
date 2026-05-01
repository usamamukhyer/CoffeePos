using DBCafeteria.Application.DTOs;
using DBCafeteria.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DBCafeteria.Api.Controllers;

[ApiController]
[Route("cart")]
public sealed class CartController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public ActionResult<CartValidationResponse> Validate(CartValidationRequest request) =>
        Ok(orderService.ValidateCart(request));
}
