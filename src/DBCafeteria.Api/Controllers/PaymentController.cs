using DBCafeteria.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DBCafeteria.Api.Controllers;

[ApiController]
[Route("payment")]
public sealed class PaymentController : ControllerBase
{
    [HttpPost]
    public ActionResult<object> Pay(PaymentRequest request) =>
        Ok(new { approved = true, transactionId = $"DUMMY-{Guid.NewGuid():N}", amount = request.Amount, currency = request.Currency });
}
