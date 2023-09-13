using ItesDemo.API.Data;
using ItesDemo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ItesDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly ApiDbContext context;

        public UsuarioController(ApiDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponse>> LoginAsync(LoginRequest login)
        {
            var userEncontrado = await context.Usuarios
                .Where(x => x.usuario == login.Usuario && x.password == login.Password)
                .FirstOrDefaultAsync();

            if (userEncontrado == null)
            {
                return NotFound();
            }
            else
            {
                // generate token
                string token = CreateAccessToken(userEncontrado);

                return Ok(new LoginResponse { Token = token});
            }
        }

        string CreateAccessToken(Usuario user)
        {
            // suppose a public key can read from appsettings
            string K = "12345678901234567890123456789012345678901234567890123456789012345678901234567890";
            // convert to bytes
            var key = Encoding.UTF8.GetBytes(K);
            // convert to symetric Security
            var skey = new SymmetricSecurityKey(key);
            // Sign de Key
            var SignedCredential = new SigningCredentials(skey, SecurityAlgorithms.HmacSha256Signature);
            // Add Claims
            var uClaims = new ClaimsIdentity(new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub,user.nombre),
                    new Claim(JwtRegisteredClaimNames.Email,user.email)
                });
            // define expiration
            var expires = DateTime.UtcNow.AddDays(1);
            // create de token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = uClaims,
                Expires = expires,
                Issuer = "ITES",
                SigningCredentials = SignedCredential,
            };
            //initiate the token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenJwt = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(tokenJwt);

            return token;
        }
    }
}
