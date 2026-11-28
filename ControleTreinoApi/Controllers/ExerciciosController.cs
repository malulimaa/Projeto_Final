using ControleTreinoApi.Dtos;
using ControleTreinoApi.Exceptions;
using ControleTreinoApi.Extensions;
using ControleTreinoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleTreinoApi.Controllers;

[ApiController]
[Authorize]
[Route("api/exercicios")]
[Produces("application/json")]
public class ExerciciosController(IExercicioService exercicioService) : ControllerBase
{
    /// <summary>Lista os exercícios da usuária logada.</summary>
    /// <response code="200">Lista de exercícios (pode ser vazia).</response>
    /// <response code="401">Token ausente ou inválido.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ExercicioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Listar()
    {
        var exercicios = await exercicioService.ListarAsync(User.ObterUsuarioId());
        return Ok(exercicios);
    }

    /// <summary>Busca um exercício da usuária logada pelo Id.</summary>
    /// <param name="id">Id do exercício.</param>
    /// <response code="200">Exercício encontrado.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Exercício não existe ou não pertence à usuária.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExercicioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(int id)
    {
        try
        {
            var exercicio = await exercicioService.ObterAsync(id, User.ObterUsuarioId());
            return Ok(exercicio);
        }
        catch (NaoEncontradoException ex)
        {
            return NotFound(new ProblemDetails { Title = ex.Message, Status = 404 });
        }
    }

    /// <summary>Cadastra um exercício para a usuária logada.</summary>
    /// <remarks>
    /// Exemplo:
    ///
    ///     POST /api/exercicios
    ///     {
    ///         "nome": "Supino reto",
    ///         "descricao": "Barra, pegada média",
    ///         "grupoMuscular": "Peito"
    ///     }
    ///
    /// Grupos aceitos: Peito, Costas, Pernas, Ombros, Braços, Abdômen ou Outro.
    /// </remarks>
    /// <response code="201">Exercício criado.</response>
    /// <response code="400">Dados inválidos (o campo é apontado na resposta).</response>
    /// <response code="401">Token ausente ou inválido.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ExercicioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Criar(ExercicioRequest request)
    {
        var exercicio = await exercicioService.CriarAsync(request, User.ObterUsuarioId());
        return CreatedAtAction(nameof(Obter), new { id = exercicio.Id }, exercicio);
    }

    /// <summary>Altera um exercício da usuária logada.</summary>
    /// <param name="id">Id do exercício.</param>
    /// <param name="request">Novos dados do exercício.</param>
    /// <response code="200">Exercício alterado.</response>
    /// <response code="400">Dados inválidos (o campo é apontado na resposta).</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Exercício não existe ou não pertence à usuária.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ExercicioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, ExercicioRequest request)
    {
        try
        {
            var exercicio = await exercicioService.AtualizarAsync(id, request, User.ObterUsuarioId());
            return Ok(exercicio);
        }
        catch (NaoEncontradoException ex)
        {
            return NotFound(new ProblemDetails { Title = ex.Message, Status = 404 });
        }
    }

    /// <summary>Exclui um exercício da usuária logada.</summary>
    /// <remarks>Exercício que está em algum treino não pode ser excluído: remova-o dos treinos antes.</remarks>
    /// <param name="id">Id do exercício.</param>
    /// <response code="204">Exercício excluído.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Exercício não existe ou não pertence à usuária.</response>
    /// <response code="409">Exercício está em uso em algum treino.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            await exercicioService.ExcluirAsync(id, User.ObterUsuarioId());
            return NoContent();
        }
        catch (NaoEncontradoException ex)
        {
            return NotFound(new ProblemDetails { Title = ex.Message, Status = 404 });
        }
        catch (ExercicioEmUsoException ex)
        {
            return Conflict(new ProblemDetails { Title = ex.Message, Status = 409 });
        }
    }
}
