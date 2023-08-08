using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using WorkingWithAPIApplication.Contracts;
using WorkingWithAPIApplication.Dto.UserDTO;
using WorkingWithAPIApplication.Entities;

namespace WorkingWithAPIApplication.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await userRepository.GetUsers();
            return Ok(users);
        }
        [HttpGet("{id}", Name = "UserById")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await userRepository.GetUser(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody]UserForCreation UserID)
        {
            var createdUser = await userRepository.CreateUser(UserID);
            return Ok(new { Id = createdUser });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,[FromBody]UserForUpdate UserU)
        {
            var User = await userRepository.GetUser(id);
            if(User == null)
                return NotFound();
            await userRepository.UpdateUser(id, UserU);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var User = await userRepository.GetUser(id);
            if (User == null)
                return NotFound();
            await userRepository.DeleteUser(id);
            return NoContent();
        }
        [HttpGet("/user/{Username}")]
        public async Task<IActionResult> GetUserForLogin(string Username, string Password)
        {
            var user = await userRepository.GetUserForLogin(Username,Password);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        private User GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new User()
                {
                    Username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                };
            }
            return null;
        }
        [HttpGet("Admins")]
        [Authorize]
        public IActionResult AdminsEndpoint()
        {
            var currentUser = GetCurrentUser();
            return Ok($"Hi {currentUser.Username}!");

        }


        

    }
}
