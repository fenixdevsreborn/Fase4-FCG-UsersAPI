using UsersAPI.Domain.Entities;
using UsersAPI.Domain.ValueObjects;

namespace UsersAPI.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(Password password);
        bool Verify(Password password, string passwordHash);
    }
}
