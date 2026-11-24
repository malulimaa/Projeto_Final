using System.ComponentModel.DataAnnotations;

namespace ControleTreinoApi.Dtos;

public class LoginRequest
{
    /// <example>seu@email.com</example>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;
    
    /// <example>suasenha123</example>
    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;
}