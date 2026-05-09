using UsersAPI.Domain.Entities;
using UsersAPI.Domain.ValueObjects;

namespace UsersAPI.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsByEmailAsync(Email email);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(Email email);
        Task AddAsync(User user);
    }
}
