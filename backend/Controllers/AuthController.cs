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
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
[HttpPost("login")]
public IActionResult Login([FromBody] dynamic body)
{
    // extrai os campos do JSON recebido
    string email = body?.email;
    string senha = body?.senha;

    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
        return BadRequest("E-mail e senha são obrigatórios.");

    // procura o usuário no banco
    var user = _context.Usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);

    if (user == null)
        return Unauthorized("Usuário ou senha incorretos.");

    // gera o token JWT
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

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
