using System.Security.Claims;

using System.IdentityModel.Tokens.Jwt;

namespace UsersAPI.Api.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException("Missing UserId claim");

            return Guid.Parse(value);
        }

        public static string GetName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("name")
                ?? throw new InvalidOperationException("Missing Name claim");
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(JwtRegisteredClaimNames.Email)
                ?? user.FindFirstValue(ClaimTypes.Email)
                ?? throw new InvalidOperationException("Missing Email claim");
        }

        public static string GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("role")
                ?? user.FindFirstValue(ClaimTypes.Role)
                ?? throw new InvalidOperationException("Missing Role claim");
        }
    }
}
