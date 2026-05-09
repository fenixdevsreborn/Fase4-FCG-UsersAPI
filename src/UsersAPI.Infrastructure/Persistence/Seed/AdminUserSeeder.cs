using UsersAPI.Domain.Entities;
using UsersAPI.Domain.ValueObjects;
using UsersAPI.Infrastructure.Security;

namespace UsersAPI.Infrastructure.Persistence.Seed
{
    public static class AdminUserSeeder
    {
        public static void Seed(UsersDbContext context)
        {
            const string adminEmail = "admin@usersapi.com";

            var email = new Email(adminEmail);

            var adminExists = context.Users.Any(u => u.Email.Value == email.Value);

            if (adminExists)
                return;

            var passwordHasher = new PasswordHasher();

            var user = new User(
                name: "Admin",
                email: email,
                passwordHash: passwordHasher.Hash(new Password("Admin123!")),
                role: User.UserRole.Admin
            );

            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
