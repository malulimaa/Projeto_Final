using ControleTreinoApi.Dtos;
using ControleTreinoApi.Exceptions;
using ControleTreinoApi.Extensions;
using ControleTreinoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleTreinoApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IUsuarioService usuarioService) : ControllerBase
{
    /// <summary>Cria a conta de uma nova usuária.</summary>
    /// <response code="201">Conta criada.</response>
    /// <response code="400">Dados inválidos (o campo é apontado na resposta).</response>
    /// <response code="409">Já existe uma conta com este e-mail.</response>
    [HttpPost("cadastro")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cadastrar(CadastroRequest request)
    {
        try
        {
            var usuario = await usuarioService.CadastrarAsync(request);
            return StatusCode(StatusCodes.Status201Created, usuario);
        }
        catch (EmailJaCadastradoException ex)
        {
            return Conflict(new ProblemDetails { Title = ex.Message, Status = 409 });
        }
    }

    /// <summary>Autentica a usuária e devolve um token.</summary>
    /// <response code="200">Token gerado.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">E-mail ou senha inválidos.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var resposta = await usuarioService.LoginAsync(request);
            return Ok(resposta);
        }
        catch (CredenciaisInvalidasException ex)
        {
            return Unauthorized(new ProblemDetails { Title = ex.Message, Status = 401 });
        }
    }

    /// <summary>Rota de teste: devolve o Id da usuária logada.</summary>
    /// <response code="200">Id da usuária lido do token.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    [Authorize]
    [HttpGet("eu")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Eu() => Ok(new { usuarioId = User.ObterUsuarioId() });
}