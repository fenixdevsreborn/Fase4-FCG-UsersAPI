using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UsersAPI.Infrastructure.Persistence;
using UsersAPI.Infrastructure.Persistence.Seed;

namespace UsersAPI.IntegrationTests.Fixtures;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Port=5433;Database=users_test_db;Username=postgres;Password=postgres",
                ["Jwt:Key"] = "users-api-integration-tests-secret-key-32chars",
                ["Jwt:Issuer"] = "UsersAPI",
                ["Jwt:Audience"] = "UsersAPI",
                ["RabbitMQ:Host"] = "localhost",
                ["RabbitMQ:Port"] = "5672",
                ["RabbitMQ:Username"] = "guest",
                ["RabbitMQ:Password"] = "guest"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<UsersDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<UsersDbContext>(options =>
                options.UseNpgsql(
                    "Host=localhost;Port=5433;Database=users_test_db;Username=postgres;Password=postgres"));

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            TestDatabaseSeeder.Seed(db);
        });
    }
}
