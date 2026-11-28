using System.ComponentModel.DataAnnotations;
using ControleTreinoApi.Dtos;

namespace ControleTreinoApi.Tests.Exercicios;

[Trait("Category", "Unit")]
public class ExercicioRequestValidacaoTests
{
    private static List<ValidationResult> Validar(ExercicioRequest request)
    {
        var erros = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), erros, validateAllProperties: true);
        return erros;
    }

    private static bool TemErroNoCampo(List<ValidationResult> erros, string campo) =>
        erros.Any(e => e.MemberNames.Contains(campo));

    // ---------- Nome: 2 a 60 caracteres ----------

    [Theory]
    [InlineData("Ab")]                      // limite mínimo
    [InlineData("Supino reto com barra")]
    public void Nome_valido_passa(string nome)
    {
        var erros = Validar(new ExercicioRequest { Nome = nome, GrupoMuscular = "Peito" });
        Assert.False(TemErroNoCampo(erros, nameof(ExercicioRequest.Nome)));
    }

    [Fact]
    public void Nome_com_60_caracteres_passa()
    {
        var erros = Validar(new ExercicioRequest { Nome = new string('a', 60), GrupoMuscular = "Peito" });
        Assert.False(TemErroNoCampo(erros, nameof(ExercicioRequest.Nome)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Nome_curto_ou_vazio_falha(string nome)
    {
        var erros = Validar(new ExercicioRequest { Nome = nome, GrupoMuscular = "Peito" });
        Assert.True(TemErroNoCampo(erros, nameof(ExercicioRequest.Nome)));
    }

    [Fact]
    public void Nome_com_61_caracteres_falha()
    {
        var erros = Validar(new ExercicioRequest { Nome = new string('a', 61), GrupoMuscular = "Peito" });
        Assert.True(TemErroNoCampo(erros, nameof(ExercicioRequest.Nome)));
    }

    // ---------- Grupo muscular: lista fechada ----------

    [Theory]
    [InlineData("Peito")]
    [InlineData("Costas")]
    [InlineData("Pernas")]
    [InlineData("Ombros")]
    [InlineData("Braços")]
    [InlineData("Abdômen")]
    [InlineData("Outro")]
    public void Grupo_muscular_permitido_passa(string grupo)
    {
        var erros = Validar(new ExercicioRequest { Nome = "Exercício", GrupoMuscular = grupo });
        Assert.False(TemErroNoCampo(erros, nameof(ExercicioRequest.GrupoMuscular)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("Gluteos")]
    [InlineData("peito")]       // diferencia maiúsculas
    [InlineData("Bracos")]      // sem acento não vale
    public void Grupo_muscular_fora_da_lista_falha(string grupo)
    {
        var erros = Validar(new ExercicioRequest { Nome = "Exercício", GrupoMuscular = grupo });
        Assert.True(TemErroNoCampo(erros, nameof(ExercicioRequest.GrupoMuscular)));
    }
}
