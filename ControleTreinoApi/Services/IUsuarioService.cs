using ControleTreinoApi.Dtos;

namespace ControleTreinoApi.Services;

public interface IUsuarioService
{
    Task<UsuarioResponse> CadastrarAsync(CadastroRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
}