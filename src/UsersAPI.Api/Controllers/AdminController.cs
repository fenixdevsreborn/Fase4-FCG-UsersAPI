using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UsersAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        [HttpGet("test")]
        public IActionResult Test()
            => Ok("Admin access granted");
    }
}
