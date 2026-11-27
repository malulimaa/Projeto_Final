using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ControleTreinoApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Devolve o Id da usuária logada, lido do token.</summary>
    public static int ObterUsuarioId(this ClaimsPrincipal user)
    {
        var valor = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(valor, out var id))
            throw new InvalidOperationException("O token não tem um Id de usuária válido.");

        return id;
    }
}