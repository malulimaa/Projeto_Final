using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ControleTreinoApi.Settings;

public class JwtOptions
{
    public const string Secao = "Jwt";

    [Required(ErrorMessage = "Jwt:Key não está configurada (user-secrets ou variável de ambiente).")]
    [MinLength(32, ErrorMessage = "Jwt:Key deve ter pelo menos 32 caracteres.")]
    public string Key { get; set; } = string.Empty;

    [Range(1, 24, ErrorMessage = "Jwt:ExpiraEmHoras deve estar entre 1 e 24.")]
    public int ExpiraEmHoras { get; set; } = 2;

    public SymmetricSecurityKey CriarChave() => new(Encoding.UTF8.GetBytes(Key));
}