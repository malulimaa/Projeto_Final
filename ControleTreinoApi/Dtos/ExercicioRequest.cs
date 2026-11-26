using System.ComponentModel.DataAnnotations;

namespace ControleTreinoApi.Dtos;

public class ExercicioRequest
{
    [Required(ErrorMessage = "O nome do exercício é obrigatório.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "O nome do exercício deve ter entre 2 e 60 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O grupo muscular é obrigatório.")]
    [RegularExpression("^(Peito|Costas|Pernas|Ombros|Braços|Abdômen|Outro)$",
        ErrorMessage = "Use: Peito, Costas, Pernas, Ombros, Braços, Abdômen ou Outro.")]
    public string GrupoMuscular { get; set; } = string.Empty;
}
