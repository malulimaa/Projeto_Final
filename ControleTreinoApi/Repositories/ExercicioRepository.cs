using ControleTreinoApi.Data;
using ControleTreinoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleTreinoApi.Repositories;

public class ExercicioRepository : IExercicioRepository
{
	private readonly AppDbContext _context;

	public ExercicioRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<List<Exercicio>> GetAllAsync(int usuarioId)
	{
		return await _context.Exercicios
			.AsNoTracking()
			.Where(e => e.UsuarioId == usuarioId)
			.OrderBy(e => e.Nome)
			.ToListAsync();
	}

	// Filtra também pela usuária: exercício de outra pessoa volta null ("como se não existisse").
	public async Task<Exercicio?> GetByIdAsync(int id, int usuarioId)
	{
		return await _context.Exercicios
			.FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == usuarioId);
	}

	public async Task AddAsync(Exercicio exercicio)
	{
		await _context.Exercicios.AddAsync(exercicio);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateAsync(Exercicio exercicio)
	{
		_context.Exercicios.Update(exercicio);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteAsync(Exercicio exercicio)
	{
		_context.Exercicios.Remove(exercicio);
		await _context.SaveChangesAsync();
	}

	public async Task<bool> EmUsoAsync(int exercicioId)
	{
		return await _context.TreinoItems.AnyAsync(i => i.ExercicioId == exercicioId);
	}
}
