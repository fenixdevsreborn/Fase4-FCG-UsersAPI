using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Security.Claims;
using System.Text;
using UsersAPI.Api.Middlewares;
using UsersAPI.Application;
using UsersAPI.Infrastructure;
using UsersAPI.Infrastructure.Persistence;
using UsersAPI.Infrastructure.Persistence.Seed;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection")!
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),

            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Users API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document, null),
            new List<string>()
        }
    });
});

var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
var rabbitMqPort = builder.Configuration["RabbitMQ:Port"] ?? "5672";
var rabbitMqUsername = builder.Configuration["RabbitMQ:Username"] ?? string.Empty;
var rabbitMqPassword = builder.Configuration["RabbitMQ:Password"] ?? string.Empty;

builder.Services.AddMassTransit(x =>
{
    //x.AddConsumer<UserCreatedIntegrationEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(host: rabbitMqHost, port: ushort.Parse(rabbitMqPort), virtualHost: "/", h =>
        {
            h.Username(rabbitMqUsername);
            h.Password(rabbitMqPassword);
        });

        // Configure explicit publication of UserCreatedIntegrationEvent
        cfg.Message<Shared.Contracts.Events.UserCreatedIntegrationEvent>(m =>
        {
            m.SetEntityName("fcg.user-created-event");
        });

        cfg.ConfigureEndpoints(context);
    });
});

// Add Health Checks
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString!, name: "postgresql")
    .AddRabbitMQ(_ =>
    {
        var factory = new RabbitMQ.Client.ConnectionFactory
        {
            HostName = rabbitMqHost,
            Port = int.Parse(rabbitMqPort),
            UserName = rabbitMqUsername,
            Password = rabbitMqPassword
        };
        return factory.CreateConnectionAsync().GetAwaiter().GetResult();
    }, name: "rabbitmq");

var app = builder.Build();

app.Logger.LogInformation(
    "Starting UsersAPI | Environment: {Env}",
    app.Environment.EnvironmentName
);

app.Logger.LogInformation(
    "RabbitMQ Host: {Host}",
    builder.Configuration["RabbitMQ:Host"]
);

if (!app.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

    db.Database.Migrate();

    // 🌱 Admin user seed (DEV / PROD)
    AdminUserSeeder.Seed(db);
}

// Enable Swagger in all environments (except Test) for Gateway aggregation
if (!app.Environment.IsEnvironment("Test"))
{
    app.UseSwagger();
    // Enable SwaggerUI in all environments (including K8s) for direct access
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UsersAPI v1");
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.Run();

public partial class Program { }
