using UsersAPI.Domain.Entities;
using UsersAPI.Domain.ValueObjects;
using UsersAPI.Infrastructure.Security;

namespace UsersAPI.Infrastructure.Persistence.Seed
{
    public static class TestDatabaseSeeder
    {
        public static void Seed(UsersDbContext context)
        {
            var passwordHasher = new PasswordHasher();

            if (!context.Users.Any(u => u.Email.Value == "test@usersapi.com"))
            {
                var user = new User(
                    name: "Test User",
                    email: new Email("test@usersapi.com"),
                    passwordHash: passwordHasher.Hash(new Password("Test!123"))
                );

                context.Users.Add(user);
                context.SaveChanges();
            }

            if (!context.Users.Any(u => u.Email.Value == "testadm@usersapi.com"))
            {

                var adm = new User(
                    name: "Test Adm User",
                    email: new Email("testadm@usersapi.com"),
                    passwordHash: passwordHasher.Hash(new Password("TstAdmin123!")),
                    role: User.UserRole.Admin
                );

                context.Users.Add(adm);
                context.SaveChanges();
            }
        }
    }
}
