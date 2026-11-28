using ControleTreinoApi.Dtos;
using ControleTreinoApi.Exceptions;
using ControleTreinoApi.Models;
using ControleTreinoApi.Repositories;

namespace ControleTreinoApi.Services;

public class ExercicioService(IExercicioRepository repository) : IExercicioService
{
    public async Task<List<ExercicioResponse>> ListarAsync(int usuarioId)
    {
        var exercicios = await repository.GetAllAsync(usuarioId);
        return exercicios.Select(ParaResponse).ToList();
    }

    public async Task<ExercicioResponse> ObterAsync(int id, int usuarioId)
    {
        var exercicio = await ObterDaUsuariaAsync(id, usuarioId);
        return ParaResponse(exercicio);
    }

    public async Task<ExercicioResponse> CriarAsync(ExercicioRequest request, int usuarioId)
    {
        var exercicio = new Exercicio
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            GrupoMuscular = request.GrupoMuscular,
            UsuarioId = usuarioId
        };

        await repository.AddAsync(exercicio);
        return ParaResponse(exercicio);
    }

    public async Task<ExercicioResponse> AtualizarAsync(int id, ExercicioRequest request, int usuarioId)
    {
        var exercicio = await ObterDaUsuariaAsync(id, usuarioId);

        exercicio.Nome = request.Nome;
        exercicio.Descricao = request.Descricao;
        exercicio.GrupoMuscular = request.GrupoMuscular;

        await repository.UpdateAsync(exercicio);
        return ParaResponse(exercicio);
    }

    public async Task ExcluirAsync(int id, int usuarioId)
    {
        // Primeiro garante que é da usuária; só depois verifica se está em uso.
        var exercicio = await ObterDaUsuariaAsync(id, usuarioId);

        if (await repository.EmUsoAsync(exercicio.Id))
            throw new ExercicioEmUsoException();

        await repository.DeleteAsync(exercicio);
    }

    // Não existe ou é de outra usuária: mesma resposta, "como se não existisse".
    private async Task<Exercicio> ObterDaUsuariaAsync(int id, int usuarioId)
    {
        return await repository.GetByIdAsync(id, usuarioId)
            ?? throw new NaoEncontradoException("Exercício não encontrado.");
    }

    private static ExercicioResponse ParaResponse(Exercicio e) =>
        new(e.Id, e.Nome, e.Descricao, e.GrupoMuscular);
}
