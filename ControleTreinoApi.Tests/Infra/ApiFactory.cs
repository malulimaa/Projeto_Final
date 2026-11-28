using System.Net.Http.Headers;
using System.Net.Http.Json;
using ControleTreinoApi.Data;
using ControleTreinoApi.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ControleTreinoApi.Tests.Infra;

// Sobe a API inteira em memória, com SQLite em memória no lugar do banco real.
public class ApiFactory : WebApplicationFactory<Program>
{
    // O banco em memória só existe enquanto esta conexão estiver aberta.
    private readonly SqliteConnection _conexao = new("Data Source=:memory:");

    public ApiFactory()
    {
        _conexao.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // UseSetting é aplicado antes de o Program.cs ler a configuração.
        builder.UseSetting("ConnectionStrings:DefaultConnection", "Data Source=:memory:");
        builder.UseSetting("Jwt:Key", "chave-de-teste-com-mais-de-32-caracteres-123456");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_conexao));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

        return host;
    }

    // Cadastra uma usuária nova (e-mail único), faz login e devolve um cliente já com o token.
    public async Task<HttpClient> CriarClienteAutenticadoAsync()
    {
        var client = CreateClient();
        var email = $"{Guid.NewGuid():N}@teste.com";
        const string senha = "senha-segura-123";

        var cadastro = await client.PostAsJsonAsync("/api/auth/cadastro",
            new CadastroRequest { Nome = "Usuária Teste", Email = email, Senha = senha });
        cadastro.EnsureSuccessStatusCode();

        var login = await client.PostAsJsonAsync("/api/auth/login", new { email, senha });
        login.EnsureSuccessStatusCode();
        var token = (await login.Content.ReadFromJsonAsync<LoginResponse>())!.Token;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _conexao.Dispose();
    }
}
