using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _jwtKey = "segredo-super-seguro-aeg2025"; // mesma do Program.cs

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Login
        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario login)
        {
            var user = _context.Usuarios.FirstOrDefault(u =>
                u.Email == login.Email && u.Senha == login.Senha);

            if (user == null)
                return Unauthorized("Usuário ou senha incorretos.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                role = user.Role
            });
        }

        // 🔹 Cadastro de novo usuário (apenas para testes)
        [HttpPost("register")]
        public IActionResult Register([FromBody] Usuario novo)
        {
            if (_context.Usuarios.Any(u => u.Email == novo.Email))
                return BadRequest("E-mail já cadastrado.");

            _context.Usuarios.Add(novo);
            _context.SaveChanges();

            return Ok("Usuário cadastrado com sucesso!");
        }
    }
}
