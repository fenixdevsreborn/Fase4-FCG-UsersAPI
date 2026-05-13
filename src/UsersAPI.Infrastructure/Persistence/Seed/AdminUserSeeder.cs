using UsersAPI.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using UsersAPI.Domain.ValueObjects;
using UsersAPI.Infrastructure.Security;

namespace UsersAPI.Infrastructure.Persistence.Seed
{
    public static class AdminUserSeeder
    {
        public static void Seed(
            UsersDbContext context,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            var adminEmail = configuration["AdminSeed:Email"];
            var adminPassword = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                if (environment.IsDevelopment() || environment.IsEnvironment("Test"))
                {
                    adminEmail = "admin@usersapi.com";
                    adminPassword = "Admin123!";
                }
                else
                {
                    throw new InvalidOperationException(
                        "AdminSeed:Email and AdminSeed:Password must be configured outside Development/Test.");
                }
            }

            var email = new Email(adminEmail);

            var adminExists = context.Users.Any(u => u.Email.Value == email.Value);

            if (adminExists)
                return;

            var passwordHasher = new PasswordHasher();

            var user = new User(
                name: "Admin",
                email: email,
                passwordHash: passwordHasher.Hash(new Password(adminPassword)),
                role: User.UserRole.Admin
            );

            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
