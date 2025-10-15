using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [StringLength(150, ErrorMessage = "O e-mail pode ter no máximo 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(4, ErrorMessage = "A senha deve ter pelo menos 4 caracteres.")]
        public string Senha { get; set; } = string.Empty;

        // use minúsculas para bater com o front (admin/user)
        [Required(ErrorMessage = "A role é obrigatória.")]
        [RegularExpression("^(admin|user)$", ErrorMessage = "Role deve ser 'admin' ou 'user'.")]
        public string Role { get; set; } = "user";
    }
}
