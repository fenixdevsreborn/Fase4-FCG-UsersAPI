using UsersAPI.Application.Common;
using UsersAPI.Application.DTOs.CreateUser;

namespace UsersAPI.Application.UseCases.CreateUser
{
    public interface ICreateUserUseCase
    {
        Task<Result<CreateUserResponse>> ExecuteAsync(CreateUserRequest request);
    }
}