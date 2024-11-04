using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Models;

public class Participante
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do participante é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "O Sobrenome do participante é obrigatório")]
    [StringLength(100, ErrorMessage = "O sobrenome pode ter no máximo 100 caracteres")]
    public string? Sobrenome { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "O e-mail deve ser válido")]
    public string? Email { get; set; }

    // Chave estrangeira
    public string? UsuarioId { get; set; } // Para relacionar com o Usuario
    public Usuario? Usuario { get; set; } // Navegação para o Usuario

    // Relacionamento com Inscricao
    public ICollection<Inscricao>? Inscricoes { get; set; }
}
