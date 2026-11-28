# Controle de Treinos API

API REST para mulheres que treinam sozinhas registrarem cargas e repetições e consultarem a própria evolução. Não há front-end: o cliente é o Swagger.

## Como testar

### Pré-requisitos

- .NET 10 SDK
- Ferramenta do Entity Framework: `dotnet tool install --global dotnet-ef`

### Testes automatizados

Os testes não dependem de banco publicado nem de user-secrets: os testes de endpoint sobem a API em memória com SQLite em memória.

Na raiz do repositório:

```bash
dotnet test -c Release
```

Para rodar só um tipo de teste:

```bash
dotnet test -c Release --filter "Category=Unit"
```

```bash
dotnet test -c Release --filter "Category=Integration"
```

- `Unit`: regras de negócio, testadas no Service com repositório fake.
- `Integration`: endpoints, verificando código de resposta e formato dos dados.

### Rodar a API localmente

Senhas, strings de conexão e a chave do token nunca vão para o repositório. Configure com user-secrets, a partir da pasta `ControleTreinoApi`:

```bash
cd ControleTreinoApi
```

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=controle-treino.db"
```

```bash
dotnet user-secrets set "Jwt:Key" "$(openssl rand -hex 32)"
```

A `Jwt:Key` precisa ter pelo menos 32 caracteres.

Crie o banco com as migrations e suba a API:

```bash
dotnet ef database update
```

```bash
dotnet run --launch-profile http
```

O Swagger fica em http://localhost:5049/swagger.

### Testar pelo Swagger

Autenticação:

1. `POST /api/auth/cadastro` com nome, e-mail e senha (mínimo de 8 caracteres): deve responder 201.
2. `POST /api/auth/login` com o mesmo e-mail e senha: copie o `token` da resposta.
3. Clique em **Authorize** (cadeado no topo), cole o token e confirme. Sem token, as rotas protegidas respondem 401.

Exercícios (`/api/exercicios`):

1. `POST` com `{"nome": "Supino reto", "grupoMuscular": "Peito"}`: deve responder 201 com o header `Location`.
2. `POST` com `"nome": "S"` ou `"grupoMuscular": "Gluteos"`: deve responder 400, apontando o campo em `errors`. Grupos aceitos: Peito, Costas, Pernas, Ombros, Braços, Abdômen ou Outro.
3. `GET /api/exercicios`: lista só os exercícios da usuária logada.
4. `GET`, `PUT` e `DELETE` em `/api/exercicios/{id}`: devem responder 200, 200 e 204.
5. Isolamento: cadastre uma segunda usuária, faça login com ela, clique em **Authorize** com o novo token e tente acessar o exercício da primeira. Deve responder 404, como se ele não existisse.
6. Exercício que está em algum treino não pode ser excluído: o `DELETE` responde 409 pedindo para removê-lo dos treinos antes. Enquanto não houver endpoint de treinos, esse caso é verificado pelos testes automatizados.
