using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UsersAPI.Infrastructure.Persistence;
using UsersAPI.Infrastructure.Persistence.Seed;

namespace UsersAPI.IntegrationTests.Fixtures;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private const string TestJwtKey = "users-api-integration-tests-secret-key-32chars";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=test;Database=users_test_db;Username=test;Password=test",
                ["Jwt:Key"] = TestJwtKey,
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
            var databaseName = $"users-api-tests-{Guid.NewGuid():N}";

            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DbContextOptions<UsersDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<UsersDbContext>>();
            services.RemoveAll<IDatabaseProvider>();

            services.AddDbContext<UsersDbContext>(options =>
                options.UseInMemoryDatabase(databaseName));

            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters.ValidIssuer = "UsersAPI";
                    options.TokenValidationParameters.ValidAudience = "UsersAPI";
                    options.TokenValidationParameters.IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtKey));
                    options.TokenValidationParameters.RoleClaimType = "role";
                    options.TokenValidationParameters.NameClaimType = "name";
                });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

            db.Database.EnsureCreated();

            TestDatabaseSeeder.Seed(db);
        });
    }
}
