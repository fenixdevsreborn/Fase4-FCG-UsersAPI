using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersAPI.Api.Common.Extensions;
using UsersAPI.Application.DTOs.CreateUser;
using UsersAPI.Application.UseCases.CreateUser;

namespace UsersAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ICreateUserUseCase _createUserUseCase;

    public UsersController(ICreateUserUseCase createUserUseCase)
    {
        _createUserUseCase = createUserUseCase;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var result = await _createUserUseCase.ExecuteAsync(request);

        if (!result.IsSuccess)
        {
            return result.Error!.Code switch
            {
                "validation_error" => BadRequest(result.Error),
                "domain_validation_error" => BadRequest(result.Error),
                "email_already_exists" => Conflict(result.Error),
                _ => StatusCode(500, result.Error)
            };
        }

        return CreatedAtAction(
            nameof(Create),
            new { id = result.Value!.UserId },
            result.Value
        );
    }

    /// <summary>
    /// Retorna informações do usuário autenticado
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        try
        {
            var response = new UserInfoResponse
            {
                UserId = User.GetUserId(),
                Name = User.GetName(),
                Email = User.GetEmail(),
                Role = User.GetRole()
            };

            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return Unauthorized(new
            {
                Message = "Invalid token: missing required claims"
            });
        }
    }

}
