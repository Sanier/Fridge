using Fridge.Domain.Models;
using Fridge.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fridge.API.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserModel user)
        {
            var response = await _userService.Create(user);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                return Ok(new { description = response.Description });

            return BadRequest(new { description = response.Description });
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(UserModel user)
        {
            var authUser = await _userService.Authenticate(user);

            if (authUser.StatusCode != System.Net.HttpStatusCode.Accepted)
                return Unauthorized();

            return Ok(new { Token = authUser });
        }

        //Нужно повесить лок и роли по типу админа и т.д.
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _userService.GetUsers();

            return Json(new { data = response.Data });
        }
    }
}
