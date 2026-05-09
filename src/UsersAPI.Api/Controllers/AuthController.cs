using Microsoft.AspNetCore.Mvc;
using UsersAPI.Application.DTOs.Auth.Login;

namespace UsersAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly LoginHandler _handler;

    public AuthController(LoginHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _handler.HandleAsync(request);

        if (!result.Success)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = result.Error
            });
        }

        return Ok(new LoginResponse { AccessToken = result.Token! });
    }
}
