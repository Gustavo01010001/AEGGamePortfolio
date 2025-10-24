using backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Verifica e obtém connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada. Configure-a em appsettings.json.");
}

// 🔹 Conexão com o banco MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// 🔹 Controllers, Swagger e CORS
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Permite reconhecer propriedades independentemente de maiúsculas/minúsculas
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});

// 🔹 JWT (pega chave do appsettings.json) — valida existência da chave
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Chave JWT não encontrada. Adicione 'Jwt:Key' em appsettings.json.");
}
var key = Encoding.ASCII.GetBytes(jwtKey);

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

// 🔹 Pipeline do aplicativo
app.UseCors("AllowAll");
app.UseStaticFiles();

// Exibe Swagger — se preferir só em Development, envolva em if (app.Environment.IsDevelopment()) { ... }
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// 🔹 Rota raiz -> redireciona para Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// 🔹 Controllers
app.MapControllers();
//dotnet run --urls http://localhost:5000
// 🔹 Executa o servidor
app.Run();
/*
💡 Lembretes:
1️⃣ appsettings.json precisa ter:
*/
