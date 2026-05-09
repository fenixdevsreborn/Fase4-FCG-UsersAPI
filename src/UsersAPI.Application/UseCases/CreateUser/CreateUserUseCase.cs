using UsersAPI.Application.Abstractions;
using UsersAPI.Application.Common;
using UsersAPI.Application.DTOs.CreateUser;
using UsersAPI.Application.Interfaces;
using UsersAPI.Application.Validators;
using UsersAPI.Domain.Entities;
using UsersAPI.Domain.ValueObjects;

namespace UsersAPI.Application.UseCases.CreateUser;

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly CreateUserValidator _validator = new();

    public CreateUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IDomainEventDispatcher eventDispatcher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result<CreateUserResponse>> ExecuteAsync(CreateUserRequest request)
    {
        var validation = _validator.Validate(request);

        if (!validation.IsValid)
        {
            return Result<CreateUserResponse>.Failure(
                "validation_error",
                "Invalid request data"
            );
        }

        Email email;
        Password password;

        try
        {
            email = new Email(request.Email);
            password = new Password(request.Password);
        }
        catch (ArgumentException ex)
        {
            return Result<CreateUserResponse>.Failure(
                "domain_validation_error",
                ex.Message
            );
        }

        if (await _userRepository.ExistsByEmailAsync(email))
        {
            return Result<CreateUserResponse>.Failure(
                "email_already_exists",
                "Email already in use"
            );
        }

        var passwordHash = _passwordHasher.Hash(password);

        var user = new User(
            request.Name,
            email,
            passwordHash
        );

        await _userRepository.AddAsync(user);
        await _eventDispatcher.DispatchAsync(user.DomainEvents);
        user.ClearDomainEvents();

        return Result<CreateUserResponse>.Success(
            new CreateUserResponse
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email.Value
            }
        );
    }
}
