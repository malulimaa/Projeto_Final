using ControleTreinoApi.Models;

namespace ControleTreinoApi.Repositories;

public interface IUsuarioRepository
{
    Task<bool> EmailExisteAsync(string email);
    Task<Usuario> AdicionarAsync (Usuario usuario);
    Task<Usuario?> ObterPorEmailAsync(string email);
}