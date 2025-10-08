using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // 🔹 Tabelas do banco
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!; // 👈 adiciona essa linha
    }
}
