namespace backend.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Status { get; set; } = "Em Desenvolvimento"; // ou "Lançado"
        public string? ImagemUrl { get; set; }
        public string? DemoUrl { get; set; }
    }
}
