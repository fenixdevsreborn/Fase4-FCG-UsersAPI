using UsersAPI.Application.DTOs.CreateUser;
using UsersAPI.Application.Validation;

namespace UsersAPI.Application.Validators;

public class CreateUserValidator
{
    public ValidationResult Validate(CreateUserRequest request)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(request.Name))
            result.Add(nameof(request.Name), "Name is required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            result.Add(nameof(request.Email), "Email is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            result.Add(nameof(request.Password), "Password is required.");

        return result;
    }
}
