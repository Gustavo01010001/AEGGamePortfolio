using backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Conexão com o banco MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// 🔹 Controllers, Swagger e CORS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});

// 🔹 Configuração de autenticação com JWT
var key = Encoding.ASCII.GetBytes("segredo-super-seguro-aeg2025"); // <-- chave secreta
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// 🔹 Construção do app
var app = builder.Build();

// 🔹 Middleware de CORS e arquivos estáticos (para imagens)
app.UseCors("AllowAll");
app.UseStaticFiles();

// 🔹 Swagger
app.UseSwagger();
app.UseSwaggerUI();

// 🔹 Middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Rota raiz -> redireciona para Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// 🔹 Controllers
app.MapControllers();

// 🔹 Executa o servidor
app.Run();



// 💡 Comando para rodar o backend:
// dotnet run --urls http://localhost:5000
