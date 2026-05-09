using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersAPI.Application.DTOs.Auth.Login
{
    public sealed class LoginResponse
    {
        public string AccessToken { get; init; } = default!;
        public DateTime ExpiresAt { get; init; }
    }
}
