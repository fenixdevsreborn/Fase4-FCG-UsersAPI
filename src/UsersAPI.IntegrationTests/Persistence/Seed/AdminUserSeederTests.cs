using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using UsersAPI.Domain.Entities;
using UsersAPI.Domain.ValueObjects;
using UsersAPI.Infrastructure.Persistence;
using UsersAPI.Infrastructure.Persistence.Seed;

namespace UsersAPI.IntegrationTests.Persistence.Seed;

public sealed class AdminUserSeederTests
{
    [Fact]
    public void Seed_should_keep_existing_admin_account_and_promote_it_when_needed()
    {
        var options = CreateOptions();
        const string adminEmail = "admin@usersapi.com";
        const string originalPasswordHash = "already-hashed-password";

        using var context = new UsersDbContext(options);
        context.Users.Add(new User(
            "Admin",
            new Email(adminEmail),
            originalPasswordHash,
            User.UserRole.User
        ));
        context.SaveChanges();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminSeed:Email"] = adminEmail
            })
            .Build();

        AdminUserSeeder.Seed(
            context,
            configuration,
            new TestHostEnvironment(Environments.Production)
        );

        var user = context.Users.Single(u => u.Email.Value == adminEmail);

        user.Role.Should().Be(User.UserRole.Admin);
        user.PasswordHash.Should().Be(originalPasswordHash);
        context.Users.Count(u => u.Email.Value == adminEmail).Should().Be(1);
    }

    [Fact]
    public void Seed_should_require_password_to_create_initial_admin_outside_development()
    {
        using var context = new UsersDbContext(CreateOptions());
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminSeed:Email"] = "admin@usersapi.com"
            })
            .Build();

        var act = () => AdminUserSeeder.Seed(
            context,
            configuration,
            new TestHostEnvironment(Environments.Production)
        );

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("AdminSeed:Password must be configured*");
    }

    private static DbContextOptions<UsersDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase($"admin-seeder-tests-{Guid.NewGuid():N}")
            .Options;
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public TestHostEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "UsersAPI.IntegrationTests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
