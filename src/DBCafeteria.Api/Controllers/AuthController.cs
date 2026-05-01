using DBCafeteria.Application.DTOs;
using DBCafeteria.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DBCafeteria.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await authService.LoginAsync(request, cancellationToken)); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("signup")]
    public async Task<ActionResult<AuthResponse>> Signup(SignupRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await authService.SignupAsync(request, cancellationToken)); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("guest")]
    public async Task<ActionResult<AuthResponse>> Guest(GuestRequest request, CancellationToken cancellationToken) =>
        Ok(await authService.GuestAsync(request, cancellationToken));
}
