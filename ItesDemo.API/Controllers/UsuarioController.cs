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
        private readonly IConfiguration configuration;

        public UsuarioController(ApiDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
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
                string token = GenerarToken(userEncontrado);

                return Ok(new LoginResponse { Token = token});
            }
        }

        string GenerarToken(Usuario usuario)
        {
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
            var signingCredentials = new SigningCredentials(
                                    new SymmetricSecurityKey(key),
                                    SecurityAlgorithms.HmacSha256Signature
                                );

            // Add Claims
            var subject = new ClaimsIdentity(new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub,usuario.nombre),
                    new Claim(JwtRegisteredClaimNames.Email,usuario.email)
                });
            // define expiration
            var expires = DateTime.UtcNow.AddDays(7);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Expires = DateTime.UtcNow.AddMinutes(10),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return jwtToken;
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
