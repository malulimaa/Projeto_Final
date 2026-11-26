using ControleTreinoApi.Dtos;
using ControleTreinoApi.Exceptions;
using ControleTreinoApi.Models;
using ControleTreinoApi.Repositories;
using JwtBearer.Services;
using Microsoft.AspNetCore.Identity;

namespace ControleTreinoApi.Services;

public class UsuarioService(
    IUsuarioRepository repository, 
    IPasswordHasher<Usuario> hasher,
    ITokenService tokenService) : IUsuarioService
{
    private static readonly Usuario UsuarioFicticio = new() {Nome = "-", Email = "-", SenhaHash = "-"};
    private static readonly string HashFicticio = new PasswordHasher<Usuario>().HashPassword(UsuarioFicticio, Guid.NewGuid().ToString("N"));

    public async Task<UsuarioResponse> CadastrarAsync(CadastroRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await repository.EmailExisteAsync(email))
            throw new EmailJaCadastradoException();

        var usuario = new Usuario { Nome = request.Nome.Trim(), Email = email };
        usuario.SenhaHash = hasher.HashPassword(usuario, request.Senha);

        await repository.AdicionarAsync(usuario);
        return new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var usuario = await repository.ObterPorEmailAsync(email);

        var resultado = hasher.VerifyHashedPassword(usuario ?? UsuarioFicticio, usuario?.SenhaHash ?? HashFicticio, request.Senha);
        
        if (usuario is null || resultado == PasswordVerificationResult.Failed)
            throw new CredenciaisInvalidasException();

        return new LoginResponse(tokenService.Generate(usuario));
    }
}