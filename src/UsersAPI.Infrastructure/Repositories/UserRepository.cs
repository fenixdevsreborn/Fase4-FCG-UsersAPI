using Microsoft.EntityFrameworkCore;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Entities;
using UsersAPI.Domain.ValueObjects;
using UsersAPI.Infrastructure.Persistence;

namespace UsersAPI.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;

    public UserRepository(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByEmailAsync(Email email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.Value == email.Value);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(Email email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email.Value);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
