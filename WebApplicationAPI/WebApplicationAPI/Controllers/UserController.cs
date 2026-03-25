using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.DTO;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            if (id < 0)
            {
                return BadRequest("ID inválido");
            }

            var user = new { Id = id, Name = "Lucas Alves", Email = "lucas@gmail.com"};

            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = new[]
            {
                new { Id = 1, Name = "Lucas", Email = "lucas@escanor.com"},
                new { Id = 2, Name = "Alves", Email = "alves@escanor.com"}
            };

            return Ok(users);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDTO user)
        {
            if (string.IsNullOrEmpty(user.Name))
            {
                return BadRequest("Faltou o nome aqui");
            }

            var createdUser = new { Id = 3, Name = user.Name, Email = user.Email };

            return Created($"/User/{createdUser.Id}", createdUser);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserDTO updatedUser)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido");
            }

            return Ok(new { Id = id, Name = updatedUser.Name, Email = updatedUser.Email });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido");
            }

            return NoContent();
        }
    }
}

