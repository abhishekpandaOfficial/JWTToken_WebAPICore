using JWT_WebAPICore.Context;
using JWT_WebAPICore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_WebAPICore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JWTTokenController : ControllerBase
    {
        public IConfiguration _configuration;
        public readonly ApplicationDBContext _applicationDBContext;
        public JWTTokenController(IConfiguration configuration, ApplicationDBContext applicationDBContext)
        {
            _configuration = configuration;
           _applicationDBContext = applicationDBContext;    
        }


        [HttpPost]
        public  async Task<IActionResult> Post(User user)
        {
            if(user != null && user.UserName !=null && user.Password !=null)
            {
                var userData = await GetUser(user.UserName, user.Password);
                var jwt = _configuration.GetSection("Jwt").Get<Jwt>();
                if(user != null)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, DateTime.UtcNow.ToString()),
                        new Claim("Id", user.UserId.ToString()),
                        new Claim("UserName", user.UserName),
                        new Claim("Password",user.Password)

                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.key));
                    var signIn = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        jwt.Issuer,
                        jwt.Audience,
                        claims,
                        expires:DateTime.Now.AddMinutes(20),
                        signingCredentials:signIn
                        );

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }

            }
            else
            {
                return BadRequest("Invalid Credentials ! Please Try Again !!!");
            }

            return Ok();
        }

        [HttpGet]
        public async Task<User> GetUser(string userName, string password)
        {
            return await _applicationDBContext.User.FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);
        }
    }
}
