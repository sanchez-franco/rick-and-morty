using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RickMortyApi.Database;
using RickMortyApi.Interfaces;
using RickMortyApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RickMortyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly string _secretKey;
        private readonly IUnitOfWork _dbContext;

        public AccountController(JwtSettings settings, SQLUnitOfWork dbContext)
        {
            _secretKey = settings.Secret;
            _dbContext = dbContext;
        }

        [HttpPost("/LogIn")]
        public ActionResult<string> Post([FromBody] AccountDto item)
        {
            if (item == null || string.IsNullOrEmpty(item.Password) || string.IsNullOrEmpty(item.Email))
            {
                return StatusCode(500, "prueba email");
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(item.Email);
                if (addr.Address != item.Email)
                {
                    return StatusCode(500, "Email inválido");
                }
            }
            catch
            {
                return StatusCode(500, "Email inválido");
            }
            var user = _dbContext.Accounts
                .FirstOrDefault(a => a.Email == item.Email && a.Password == item.Password);
            if (user == null)
            {
                return StatusCode(401, "Credenciales inválidas");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return Ok(jwtToken);
        }

        [HttpPost("/SignUp")]
        public ActionResult<bool> SignUp([FromBody] AccountDto item)
        {
            if (string.IsNullOrEmpty(item.Password) || string.IsNullOrEmpty(item.Email))
            {
                return StatusCode(500, "prueba");
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(item.Email);
                if (addr.Address != item.Email)
                {
                    return StatusCode(500, "Email inválido");
                }
            }
            catch
            {
                return StatusCode(500, "Email inválido");
            }
            if (_dbContext.Accounts.Any(a => a.Email == item.Email))
            {
                return StatusCode(500, "El email ya está registrado.");
            }
            _dbContext.Accounts.Add(new AccountDb { Email = item.Email, Password = item.Password });
            _dbContext.SaveChanges();

            return Ok(true);
        }

        [Authorize]
        [HttpDelete]
        public ActionResult<string> DeleteUser()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return StatusCode(500, "No se encontró el email en el token.");
            }
            var user = _dbContext.Accounts.FirstOrDefault(a => a.Email == email);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }
            _dbContext.Accounts.Remove(user);
            _dbContext.SaveChanges();
            return Ok("Usuario eliminado correctamente.");
        }
    }
}
