using ControleTreinoApi.Dtos;
using ControleTreinoApi.Exceptions;
using ControleTreinoApi.Services;
using ControleTreinoApi.Tests.Fakes;

namespace ControleTreinoApi.Tests.Exercicios;

[Trait("Category", "Unit")]
public class ExercicioServiceTests
{
    private const int Ana = 1;
    private const int Bia = 2;

    private readonly FakeExercicioRepository _repository = new();
    private readonly ExercicioService _service;

    public ExercicioServiceTests()
    {
        _service = new ExercicioService(_repository);
    }

    // ---------- Criar ----------

    [Fact]
    public async Task Criar_associa_exercicio_a_usuaria_logada()
    {
        var request = new ExercicioRequest { Nome = "Supino", Descricao = "Reto", GrupoMuscular = "Peito" };

        var criado = await _service.CriarAsync(request, Ana);

        Assert.True(criado.Id > 0);
        Assert.Equal("Supino", criado.Nome);
        Assert.Single(await _service.ListarAsync(Ana));
        Assert.Empty(await _service.ListarAsync(Bia));
    }

    // ---------- Isolamento entre usuárias ----------

    [Fact]
    public async Task Listar_devolve_apenas_exercicios_da_usuaria()
    {
        _repository.Semear("Supino", "Peito", Ana);
        _repository.Semear("Remada", "Costas", Ana);
        _repository.Semear("Agachamento", "Pernas", Bia);

        var lista = await _service.ListarAsync(Ana);

        Assert.Equal(2, lista.Count);
        Assert.DoesNotContain(lista, e => e.Nome == "Agachamento");
    }

    [Fact]
    public async Task Obter_exercicio_de_outra_usuaria_lanca_nao_encontrado()
    {
        var daBia = _repository.Semear("Agachamento", "Pernas", Bia);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => _service.ObterAsync(daBia.Id, Ana));
    }

    [Fact]
    public async Task Atualizar_exercicio_de_outra_usuaria_lanca_nao_encontrado_e_nao_altera()
    {
        var daBia = _repository.Semear("Agachamento", "Pernas", Bia);
        var request = new ExercicioRequest { Nome = "Alterado", GrupoMuscular = "Outro" };

        await Assert.ThrowsAsync<NaoEncontradoException>(() => _service.AtualizarAsync(daBia.Id, request, Ana));
        Assert.Equal("Agachamento", daBia.Nome);
    }

    [Fact]
    public async Task Obter_exercicio_inexistente_lanca_nao_encontrado()
    {
        await Assert.ThrowsAsync<NaoEncontradoException>(() => _service.ObterAsync(999, Ana));
    }

    // ---------- Atualizar ----------

    [Fact]
    public async Task Atualizar_altera_os_dados_do_exercicio()
    {
        var supino = _repository.Semear("Supino", "Peito", Ana);
        var request = new ExercicioRequest { Nome = "Supino inclinado", Descricao = "30 graus", GrupoMuscular = "Peito" };

        var atualizado = await _service.AtualizarAsync(supino.Id, request, Ana);

        Assert.Equal("Supino inclinado", atualizado.Nome);
        Assert.Equal("30 graus", atualizado.Descricao);
    }

    // ---------- Excluir ----------

    [Fact]
    public async Task Excluir_exercicio_sem_uso_remove()
    {
        var supino = _repository.Semear("Supino", "Peito", Ana);

        await _service.ExcluirAsync(supino.Id, Ana);

        Assert.Empty(await _service.ListarAsync(Ana));
    }

    [Fact]
    public async Task Excluir_exercicio_em_uso_lanca_em_uso_e_nao_remove()
    {
        var supino = _repository.Semear("Supino", "Peito", Ana);
        _repository.MarcarEmUso(supino.Id);

        await Assert.ThrowsAsync<ExercicioEmUsoException>(() => _service.ExcluirAsync(supino.Id, Ana));
        Assert.False(_repository.DeleteFoiChamado);
    }

    [Fact]
    public async Task Excluir_exercicio_em_uso_de_outra_usuaria_responde_nao_encontrado_e_nao_em_uso()
    {
        // Não pode revelar que o exercício existe e está em um treino.
        var daBia = _repository.Semear("Agachamento", "Pernas", Bia);
        _repository.MarcarEmUso(daBia.Id);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => _service.ExcluirAsync(daBia.Id, Ana));
        Assert.False(_repository.DeleteFoiChamado);
    }
}
