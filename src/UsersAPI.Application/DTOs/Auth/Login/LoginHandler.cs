using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.ValueObjects;
using UsersAPI.Application.Common.Exceptions;

namespace UsersAPI.Application.DTOs.Auth.Login
{
    public sealed class LoginHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwt;

        public LoginHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwt)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwt = jwt;
        }

        public async Task<LoginResult> HandleAsync(LoginRequest request)
        {
            var email = new Email(request.Email);

            var user = await _userRepository.GetByEmailAsync(email);

            if (user is null)
                return LoginResult.InvalidCredentials();

            var pwd = new Password(request.Password, true);

            var isValid = _passwordHasher.Verify(pwd, user.PasswordHash);

            if (!isValid)
                return LoginResult.InvalidCredentials();

            var token = _jwt.Generate(user);

            return LoginResult.Ok(token);
        }
    }
}
