using Microsoft.AspNetCore.Mvc;

using Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly DBContext context;

        public LoginController(IConfiguration configuration, DBContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        [HttpPost("User Verification")]
        public IActionResult User_Verify([FromBody] Credential data)
        {
            try
            {
                var user = context.Users.FirstOrDefault(e => e.Email == data.Email);
                if (user == null)
                {
                    return BadRequest("Invalid Email");
                }
                else
                {
                    if (user.Password == data.Password)
                    {
                       
                        var JWTtoken = Authenticate(data.Email);
                        return Ok(new { id = user.Id,role=user.Role, token = JWTtoken });
                    }
                    return BadRequest("Invalid Password");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("User_Email_Exists/{email}")]
        public IActionResult User_Exists(string email)
        {
            try
            {
                var user = context.Users.FirstOrDefault(x => x.Email==email);
                if (user == null)
                {
                    return Ok(false);
                }
                return Ok(true);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Vendor Verification")]
        public IActionResult Vendor_Verify([FromBody] Credential data)
        {
            try
            {
                var user = context.Vendors.FirstOrDefault(e => e.Email == data.Email);
                if (user == null)
                {
                    return BadRequest("Invalid Email");
                }
                else
                {
                    if (user.Password == data.Password)
                    {

                        var JWTtoken = Authenticate(data.Email);
                        return Ok(new { id = user.Id,role="Vendor", token = JWTtoken });
                    }
                    return BadRequest("Invalid Password");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("Vendor_Email_Exists/{email}")]
        public IActionResult Vendor_Exists(string email)
        {
            try
            {
                var user = context.Vendors.Where(x => x.Email.Equals(email));
                if (user.Any())
                {
                    return Ok(true);
                }
                return Ok(false);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        private object Authenticate(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, email),
                   // new Claim(ClaimTypes.Role, "admin")
                }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }



        [HttpPatch("ChangePassword/{id}")]
        public IActionResult ChangePassword(int id,string password)
        {
            var user = context.Users.Find(id);
            if (user==null)
            {
                return StatusCode(404, "User Not Found");
            }
            else
            {
                user.Password = password;
                context.SaveChanges();
                return Ok(user);
            }
            
        }
    }
    public class Credential
    {

        public string Email { get; set; }
        public string Password { get; set; }

    }
}
