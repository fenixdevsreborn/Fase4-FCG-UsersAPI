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

            if (string.IsNullOrWhiteSpace(adminEmail))
            {
                if (environment.IsDevelopment() || environment.IsEnvironment("Test"))
                {
                    adminEmail = "admin@usersapi.com";
                }
                else
                {
                    throw new InvalidOperationException(
                        "AdminSeed:Email must be configured outside Development/Test.");
                }
            }

            var email = new Email(adminEmail);

            var existingAdmin = context.Users.FirstOrDefault(u => u.Email.Value == email.Value);

            if (existingAdmin is not null)
            {
                existingAdmin.PromoteToAdmin();
                context.SaveChanges();
                return;
            }

            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                if (environment.IsDevelopment() || environment.IsEnvironment("Test"))
                {
                    adminPassword = "Admin123!";
                }
                else
                {
                    throw new InvalidOperationException(
                        "AdminSeed:Password must be configured to create the initial admin user outside Development/Test.");
                }
            }

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
