using ControleTreinoApi.Data;
using ControleTreinoApi.Exceptions;
using ControleTreinoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleTreinoApi.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository (AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email);
    }

    public async Task<Usuario> AdicionarAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);

        try
        {
            await _context.SaveChangesAsync();
            return usuario;
        }
        catch
        {
            _context.Entry(usuario).State = EntityState.Detached;

            if (await EmailExisteAsync(usuario.Email))
            {
                throw new EmailJaCadastradoException();
            }

            throw;
        }
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }
}