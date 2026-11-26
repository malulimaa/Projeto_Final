using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ControleTreinoApi.Models;
using ControleTreinoApi.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JwtBearer.Services;

public interface ITokenService
{
    string Generate(Usuario usuario);
}

public class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions jwt = options.Value;
    public string Generate(Usuario usuario)
    {

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim []
            {
                new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, usuario.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(jwt.ExpiraEmHoras),
            SigningCredentials = new SigningCredentials(jwt.CriarChave(), SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }
}