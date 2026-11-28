using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ControleTreinoApi.Data;
using ControleTreinoApi.Dtos;
using ControleTreinoApi.Models;
using ControleTreinoApi.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;

namespace ControleTreinoApi.Tests.Exercicios;

[Trait("Category", "Integration")]
public class ExerciciosEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string Rota = "/api/exercicios";

    private static async Task<ExercicioResponse> CriarExercicioAsync(HttpClient client, string nome = "Supino")
    {
        var resposta = await client.PostAsJsonAsync(Rota,
            new ExercicioRequest { Nome = nome, Descricao = "Barra", GrupoMuscular = "Peito" });
        resposta.EnsureSuccessStatusCode();
        return (await resposta.Content.ReadFromJsonAsync<ExercicioResponse>())!;
    }

    // Ainda não há endpoint de treinos: coloca o exercício em um treino direto pelo banco.
    private void ColocarEmTreino(int exercicioId)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var exercicio = db.Exercicios.Find(exercicioId)!;
        var treino = new Treino { Nome = "Treino A", UsuarioId = exercicio.UsuarioId };
        db.Treinos.Add(treino);
        db.SaveChanges();

        db.TreinoItems.Add(new TreinoItem
        {
            TreinoId = treino.Id,
            ExercicioId = exercicioId,
            Ordem = 1,
            RepeticoesAlvo = 10,
            CargaSugeridaKg = 20
        });
        db.SaveChanges();
    }

    private static async Task<JsonElement> ErrosDeValidacaoAsync(HttpResponseMessage resposta)
    {
        var json = await resposta.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("errors");
    }

    // ---------- Autenticação ----------

    [Theory]
    [InlineData("GET", Rota)]
    [InlineData("GET", Rota + "/1")]
    [InlineData("DELETE", Rota + "/1")]
    public async Task Sem_token_responde_401(string metodo, string url)
    {
        var client = factory.CreateClient();

        var resposta = await client.SendAsync(new HttpRequestMessage(new HttpMethod(metodo), url));

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    // ---------- Criar ----------

    [Fact]
    public async Task Post_valido_responde_201_com_location_e_dados()
    {
        var client = await factory.CriarClienteAutenticadoAsync();

        var resposta = await client.PostAsJsonAsync(Rota,
            new ExercicioRequest { Nome = "Supino reto", Descricao = "Barra", GrupoMuscular = "Peito" });

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        var criado = await resposta.Content.ReadFromJsonAsync<ExercicioResponse>();
        Assert.NotNull(criado);
        Assert.True(criado.Id > 0);
        Assert.Equal("Supino reto", criado.Nome);
        Assert.Equal("Barra", criado.Descricao);
        Assert.Equal("Peito", criado.GrupoMuscular);
        Assert.EndsWith($"{Rota}/{criado.Id}", resposta.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Post_com_nome_invalido_responde_400_apontando_o_campo()
    {
        var client = await factory.CriarClienteAutenticadoAsync();

        var resposta = await client.PostAsJsonAsync(Rota,
            new ExercicioRequest { Nome = "S", GrupoMuscular = "Peito" });

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        var erros = await ErrosDeValidacaoAsync(resposta);
        Assert.True(erros.TryGetProperty("Nome", out _));
    }

    [Fact]
    public async Task Post_com_grupo_invalido_responde_400_apontando_o_campo()
    {
        var client = await factory.CriarClienteAutenticadoAsync();

        var resposta = await client.PostAsJsonAsync(Rota,
            new ExercicioRequest { Nome = "Elevação", GrupoMuscular = "Gluteos" });

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        var erros = await ErrosDeValidacaoAsync(resposta);
        Assert.True(erros.TryGetProperty("GrupoMuscular", out _));
    }

    // ---------- Listar e obter ----------

    [Fact]
    public async Task Get_lista_apenas_exercicios_da_usuaria()
    {
        var ana = await factory.CriarClienteAutenticadoAsync();
        var bia = await factory.CriarClienteAutenticadoAsync();
        await CriarExercicioAsync(ana, "Supino");
        await CriarExercicioAsync(ana, "Crucifixo");
        await CriarExercicioAsync(bia, "Agachamento");

        var resposta = await ana.GetAsync(Rota);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var lista = await resposta.Content.ReadFromJsonAsync<List<ExercicioResponse>>();
        Assert.NotNull(lista);
        Assert.Equal(2, lista.Count);
        Assert.DoesNotContain(lista, e => e.Nome == "Agachamento");
    }

    [Fact]
    public async Task Get_por_id_responde_200_com_dados()
    {
        var client = await factory.CriarClienteAutenticadoAsync();
        var criado = await CriarExercicioAsync(client);

        var resposta = await client.GetAsync($"{Rota}/{criado.Id}");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal(criado, await resposta.Content.ReadFromJsonAsync<ExercicioResponse>());
    }

    [Fact]
    public async Task Get_por_id_inexistente_responde_404()
    {
        var client = await factory.CriarClienteAutenticadoAsync();

        var resposta = await client.GetAsync($"{Rota}/999999");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    // ---------- Isolamento ----------

    [Fact]
    public async Task Exercicio_de_outra_usuaria_responde_404_em_get_put_e_delete()
    {
        var ana = await factory.CriarClienteAutenticadoAsync();
        var bia = await factory.CriarClienteAutenticadoAsync();
        var daAna = await CriarExercicioAsync(ana);
        var url = $"{Rota}/{daAna.Id}";

        var get = await bia.GetAsync(url);
        var put = await bia.PutAsJsonAsync(url, new ExercicioRequest { Nome = "Hackeado", GrupoMuscular = "Outro" });
        var delete = await bia.DeleteAsync(url);

        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);

        // O exercício da Ana continua intacto.
        var original = await ana.GetFromJsonAsync<ExercicioResponse>(url);
        Assert.Equal(daAna, original);
    }

    // ---------- Atualizar ----------

    [Fact]
    public async Task Put_valido_responde_200_com_dados_alterados()
    {
        var client = await factory.CriarClienteAutenticadoAsync();
        var criado = await CriarExercicioAsync(client);

        var resposta = await client.PutAsJsonAsync($"{Rota}/{criado.Id}",
            new ExercicioRequest { Nome = "Supino inclinado", Descricao = "30 graus", GrupoMuscular = "Peito" });

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var atualizado = await resposta.Content.ReadFromJsonAsync<ExercicioResponse>();
        Assert.Equal(new ExercicioResponse(criado.Id, "Supino inclinado", "30 graus", "Peito"), atualizado);
    }

    // ---------- Excluir ----------

    [Fact]
    public async Task Delete_sem_uso_responde_204_e_exercicio_some()
    {
        var client = await factory.CriarClienteAutenticadoAsync();
        var criado = await CriarExercicioAsync(client);

        var resposta = await client.DeleteAsync($"{Rota}/{criado.Id}");

        Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"{Rota}/{criado.Id}")).StatusCode);
    }

    [Fact]
    public async Task Delete_em_uso_responde_409_com_mensagem_e_nao_exclui()
    {
        var client = await factory.CriarClienteAutenticadoAsync();
        var criado = await CriarExercicioAsync(client);
        ColocarEmTreino(criado.Id);

        var resposta = await client.DeleteAsync($"{Rota}/{criado.Id}");

        Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
        var problema = await resposta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains("Remova-o dos treinos", problema.GetProperty("title").GetString());
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"{Rota}/{criado.Id}")).StatusCode);
    }
}
