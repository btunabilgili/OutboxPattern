using Microsoft.AspNetCore.Mvc;
using OutboxPattern.Models;
using OutboxPattern.Services;

namespace OutboxPattern.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost("CreateWithoutOutboxPattern")]
        public IActionResult CreateWithoutOutboxPattern([FromKeyedServices("WithoutOutboxPattern")]IUserService userService, UserCreateModel model)
        {
            var user = userService.CreateUser(model);
            return CreatedAtAction("CreateWithoutOutboxPattern", user);
        }

        [HttpPost("CreateWithOutboxPattern")]
        public IActionResult CreateWithOutboxPattern([FromKeyedServices("WithOutboxPattern")] IUserService userService, UserCreateModel model)
        {
            var user = userService.CreateUser(model);
            return CreatedAtAction("CreateWithOutboxPattern", user);
        }
    }
}
