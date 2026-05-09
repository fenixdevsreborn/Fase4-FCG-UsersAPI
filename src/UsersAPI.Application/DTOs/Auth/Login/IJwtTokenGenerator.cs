using UsersAPI.Domain.Entities;

namespace UsersAPI.Application.DTOs.Auth.Login
{
    public interface IJwtTokenGenerator
    {
        string Generate(User user);
    }
}
