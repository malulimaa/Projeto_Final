using ControleTreinoApi.Dtos;

namespace ControleTreinoApi.Services;

public interface IExercicioService
{
    Task<List<ExercicioResponse>> ListarAsync(int usuarioId);
    Task<ExercicioResponse> ObterAsync(int id, int usuarioId);
    Task<ExercicioResponse> CriarAsync(ExercicioRequest request, int usuarioId);
    Task<ExercicioResponse> AtualizarAsync(int id, ExercicioRequest request, int usuarioId);
    Task ExcluirAsync(int id, int usuarioId);
}
