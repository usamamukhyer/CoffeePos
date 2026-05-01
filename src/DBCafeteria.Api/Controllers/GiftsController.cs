using DBCafeteria.Application.DTOs;
using DBCafeteria.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DBCafeteria.Api.Controllers;

[ApiController]
[Route("gifts")]
public sealed class GiftsController(IGiftService giftService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<GiftDto>> Create([FromQuery] int orderId, [FromQuery] int? senderCustomerId, GiftRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await giftService.CreateGiftAsync(orderId, request, senderCustomerId, cancellationToken)); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GiftDto>> Get(int id, CancellationToken cancellationToken)
    {
        var gift = await giftService.GetGiftAsync(id, cancellationToken);
        return gift is null ? NotFound() : Ok(gift);
    }
}
