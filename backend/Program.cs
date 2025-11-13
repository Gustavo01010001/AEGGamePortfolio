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
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };

    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JwtEvents");
            logger.LogInformation("OnMessageReceived. Authorization header present: {HasAuth}", ctx.Request.Headers.ContainsKey("Authorization"));
            return Task.CompletedTask;
        },
        OnTokenValidated = async ctx =>
        {
            var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JwtEvents");
            try
            {
                // pega email/unique_name do token
                var email = ctx.Principal?.Identity?.Name ?? ctx.Principal?.FindFirst("unique_name")?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    logger.LogWarning("Token validated but no email claim found.");
                    ctx.Fail("No user claim.");
                    return;
                }

                // checar no banco se usuário existe/está ativo
                var db = ctx.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var user = await db.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
                if (user == null)
                {
                    logger.LogWarning("Token user not found in DB: {Email}", email);
                    ctx.Fail("User not found.");
                    return;
                }

                // opcional: validar role/flags/etc
                logger.LogInformation("Token validated and user found: {Email}", email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in OnTokenValidated");
                ctx.Fail("Token validation error");
            }
        },
        OnAuthenticationFailed = ctx =>
        {
            var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JwtEvents");
            logger.LogError(ctx.Exception, "JWT authentication failed");
            return Task.CompletedTask;
        },
        OnChallenge = ctx =>
        {
            var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JwtEvents");
            logger.LogWarning("OnChallenge called, error: {Error}, description: {Desc}", ctx.Error, ctx.ErrorDescription);
            return Task.CompletedTask;
        }
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

// 🔹 Executa o servidor
app.Run();
/*
💡 Lembretes:
1️⃣ appsettings.json precisa ter:
*/

curl.exe --% -v -H "Authorization: Bearer eyJhbGciOi..." http://localhost:5000/api/sua-rota-protegida
