using ControleTreinoApi.Models;

namespace ControleTreinoApi.Repositories;

public interface IExercicioRepository
{
	Task<List<Exercicio>> GetAllAsync(int usuarioId);
	Task<Exercicio?> GetByIdAsync(int id, int usuarioId);
	Task AddAsync(Exercicio exercicio);
	Task UpdateAsync(Exercicio exercicio);
	Task DeleteAsync(Exercicio exercicio);
	Task<bool> EmUsoAsync(int exercicioId);
}
