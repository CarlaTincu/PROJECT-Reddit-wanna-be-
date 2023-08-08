using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkingWithAPIApplication.Contracts;
using WorkingWithAPIApplication.Entities;
using WorkingWithAPIApplication.Repository;

namespace WorkingWithAPIApplication.Controllers
{
    [Route("api/Login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IUserRepository userRepository;
        public LoginController(IConfiguration config, IUserRepository userRepository)
        {
            _config = config;
            this.userRepository = userRepository;

        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] LoginUser userLogin)
        {
            var user = Authenticate(userLogin);
            if(user!= null)
            {
                var token = Generated(user);
                var options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,  
                    SameSite = SameSiteMode.Strict  
                };
                Response.Cookies.Append("jwt", token, options);

                return Ok(new { Token = token});
            }
            return NotFound("User not found!");
        }

        private string Generated(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username),
                new Claim(ClaimTypes.Email,user.Email),
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User Authenticate(LoginUser userLogin)
        {
            var currentUser = userRepository.GetUser(userLogin.UserName);
            {
                return currentUser;
            }
            return null;
        }
    }
}
