using System.ComponentModel.DataAnnotations;

namespace ControleTreinoApi.Dtos;

public class CadastroRequest
{
    /// <example>Seu Nome</example>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <example>seu@email.com</example>
    [Required(ErrorMessage = "O E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O E-mail é inválido.")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;
    
    /// <example>suasenha123</example>
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}
