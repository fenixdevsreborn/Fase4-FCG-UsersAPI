using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UsersAPI.Application.DTOs.Auth.Login;
using UsersAPI.Domain.Entities;

namespace UsersAPI.Infrastructure.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _config;
    private const int DefaultExpiresInMinutes = 60;

    public JwtTokenGenerator(IConfiguration config)
    {
        _config = config;
    }

    public string Generate(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim("name", user.Name),
            new Claim("role", user.Role.ToString())
        };

        var expiresInMinutes = int.TryParse(_config["Jwt:ExpiresInMinutes"], out var configuredMinutes)
            ? configuredMinutes
            : DefaultExpiresInMinutes;

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
