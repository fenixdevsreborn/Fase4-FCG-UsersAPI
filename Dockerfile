# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution file
COPY ["UsersAPI.slnx", "./"]

# Copy project files
COPY ["src/UsersAPI.Domain/UsersAPI.Domain.csproj", "src/UsersAPI.Domain/"]
COPY ["src/UsersAPI.Application/UsersAPI.Application.csproj", "src/UsersAPI.Application/"]
COPY ["src/UsersAPI.Infrastructure/UsersAPI.Infrastructure.csproj", "src/UsersAPI.Infrastructure/"]
COPY ["src/UsersAPI.Api/UsersAPI.Api.csproj", "src/UsersAPI.Api/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]

# Restore dependencies for source projects only (exclude tests)
RUN dotnet restore src/UsersAPI.Domain/UsersAPI.Domain.csproj && \
    dotnet restore src/UsersAPI.Application/UsersAPI.Application.csproj && \
    dotnet restore src/UsersAPI.Infrastructure/UsersAPI.Infrastructure.csproj && \
    dotnet restore src/Shared/Shared.csproj && \
    dotnet restore src/UsersAPI.Api/UsersAPI.Api.csproj

# Copy all source code
COPY . .

# Build the API project
WORKDIR "/src/src/UsersAPI.Api"
RUN dotnet build "UsersAPI.Api.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "UsersAPI.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "UsersAPI.Api.dll"]
