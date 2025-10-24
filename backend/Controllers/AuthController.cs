using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context, IConfiguration config, ILogger<AuthController> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }

        public class LoginDto
        {
            public string? Email { get; set; }
            public string? Senha { get; set; }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto body)
        {
            _logger.LogInformation("Login attempt for {Email}", body?.Email);

            if (body == null || string.IsNullOrWhiteSpace(body.Email) || string.IsNullOrWhiteSpace(body.Senha))
                return BadRequest("E-mail e senha são obrigatórios.");

            // busca case-insensitive
            var user = _context.Usuarios
                .FirstOrDefault(u => u.Email.ToLower() == body.Email.ToLower());

            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", body.Email);
                return Unauthorized("Usuário ou senha incorretos.");
            }

            // ATENÇÃO: se você usa hash, verifique com BCrypt/Verify; aqui é comparação direta
            if (user.Senha != body.Senha)
            {
                _logger.LogWarning("Invalid password for {Email}", body.Email);
                return Unauthorized("Usuário ou senha incorretos.");
            }

            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                _logger.LogError("Jwt:Key ausente nas configurações.");
                throw new InvalidOperationException("Chave JWT não configurada.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var written = tokenHandler.WriteToken(token);

            _logger.LogInformation("Login successful for {Email}", user.Email);

            return Ok(new { token = written, role = user.Role });
        }

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
