using ControleTreinoApi.Models;
using ControleTreinoApi.Repositories;

namespace ControleTreinoApi.Tests.Fakes;

// Repositório em memória: buscas filtram pela usuária, como no real.
public class FakeExercicioRepository : IExercicioRepository
{
    private readonly List<Exercicio> _exercicios = [];
    private readonly HashSet<int> _idsEmUso = [];
    private int _proximoId = 1;

    public bool DeleteFoiChamado { get; private set; }

    public Exercicio Semear(string nome, string grupo, int usuarioId)
    {
        var exercicio = new Exercicio
        {
            Id = _proximoId++,
            Nome = nome,
            Descricao = null,
            GrupoMuscular = grupo,
            UsuarioId = usuarioId
        };
        _exercicios.Add(exercicio);
        return exercicio;
    }

    public void MarcarEmUso(int exercicioId) => _idsEmUso.Add(exercicioId);

    public Task<List<Exercicio>> GetAllAsync(int usuarioId) =>
        Task.FromResult(_exercicios.Where(e => e.UsuarioId == usuarioId).ToList());

    public Task<Exercicio?> GetByIdAsync(int id, int usuarioId) =>
        Task.FromResult(_exercicios.FirstOrDefault(e => e.Id == id && e.UsuarioId == usuarioId));

    public Task AddAsync(Exercicio exercicio)
    {
        exercicio.Id = _proximoId++;
        _exercicios.Add(exercicio);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Exercicio exercicio) => Task.CompletedTask;

    public Task DeleteAsync(Exercicio exercicio)
    {
        DeleteFoiChamado = true;
        _exercicios.Remove(exercicio);
        return Task.CompletedTask;
    }

    public Task<bool> EmUsoAsync(int exercicioId) => Task.FromResult(_idsEmUso.Contains(exercicioId));
}
