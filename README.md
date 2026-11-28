# Controle de Treinos API

API REST em .NET 10 para mulheres que treinam sozinhas na academia registrarem cargas e repetições e consultarem a própria evolução. Não há front-end nesta etapa: a API é demonstrada pelo Swagger.

## Sumário

- [Problema e público](#problema-e-público)
- [MVP](#mvp)
- [Extras](#extras)
- [Tecnologias](#tecnologias)
- [Como executar localmente](#como-executar-localmente)
- [Testes automatizados](#testes-automatizados)
- [Testar pelo Swagger](#testar-pelo-swagger)
- [Diagrama das entidades](#diagrama-das-entidades)
- [Documentação da API](#documentação-da-api)

## Problema e público

### Em três frases

Mulheres que treinam sozinhas na academia, em geral iniciantes ou intermediárias, não têm um registro organizado das cargas e das repetições, e por isso não sabem de onde partir no próximo treino nem se estão evoluindo. Isso importa porque, sem uma referência objetiva de evolução, a motivação cai e os resultados estagnam. Esta API registra o que foi realmente executado em cada sessão e devolve a maior carga de cada exercício.

### Quem é a usuária

Mulheres que frequentam academia e treinam sem acompanhamento profissional. Trabalhamos com duas personas:

| Persona | Perfil | O que precisa | Principais dores |
|---|---|---|---|
| **Camila**, 24 anos | Iniciante, treina sozinha há 3 meses | Saber qual carga usar, ter um treino fácil de seguir e ver a evolução | Esquece a carga da semana anterior e perde a motivação sem resultado visível |
| **Fernanda**, 31 anos | Intermediária/avançada, treina há 5 anos, 5 vezes por semana | Consultar a maior carga, ver a evolução por semana e por mês e ajustar o treino sem perder o histórico | Dados espalhados e muito tempo digitando as mesmas informações |

Hoje, as duas usam fichas de papel ou aplicativos genéricos e confusos, sem uma visão organizada da evolução.

### Respostas às perguntas do enunciado

**Quem é a usuária? Há quanto tempo treina e o que usa hoje para anotar o treino?**
Camila treina há cerca de 3 meses e Fernanda há 5 anos. Ambas anotam em fichas de papel ou em aplicativos genéricos, e os dados ficam espalhados.

**Em que momento do treino ela sente mais falta de uma informação?**
Na hora de escolher a carga de um exercício, quando ela não lembra o que levantou na semana anterior. Esse é o momento em que ela mais sente falta de uma referência.

**Qual informação ela quer ver primeiro?**
A maior carga de cada exercício. Depois dela, vêm o histórico e a evolução por semana e por mês, que ficam como extras.

**Quais dados ela não quer digitar toda vez?**
Os exercícios e a estrutura do treino (exercícios, ordem, repetições e carga sugerida). Por isso a usuária monta o treino uma vez e, a cada sessão, só informa a carga e as repetições que realmente executou. A API também pode oferecer uma lista inicial de exercícios comuns ao criar a conta.

<!-- REVISAR: respostas 2 e 4 foram deduzidas das personas e do enunciado; confirmar com o grupo. -->

## MVP

O MVP é o menor conjunto de funcionalidades que resolve o problema de ponta a ponta, do cadastro até a consulta da maior carga. Para decidir o que entra, usamos a pergunta: **"sem esta funcionalidade, o problema continua sem solução?"**

| Funcionalidade | Entra no MVP? | Justificativa |
|---|---|---|
| Cadastro e login com token | Sim | Sem conta e sem autenticação, não há como isolar os dados de cada usuária |
| Cadastro de exercícios | Sim | O registro de carga precisa de exercícios para se referir |
| Treino com séries | Sim | É o plano que a usuária segue e a base para registrar a sessão |
| Registro de sessão | Sim | Sem registrar o que foi executado, não existe histórico nem maior carga |
| Consulta da maior carga | Sim | É a informação que a usuária mais quer ver e resolve a falta de referência |

### Regras que o MVP garante

- A maior carga vem somente do que foi registrado, nunca do plano.
- Editar o plano não altera sessões já registradas (plano e execução são independentes).
- Séries não preenchidas ficam fora da conta.
- Carga nunca é negativa (pode ser zero) e repetições vão de 1 a 100.
- Uma sessão só aceita séries do próprio treino.
- Exercício em uso em algum treino não pode ser excluído.
- Cada usuária vê apenas os próprios dados.
- E-mail é único e a senha tem no mínimo 8 caracteres, guardada apenas como hash.

## Extras

Ficam para depois, porque sem eles o problema principal já está resolvido:

- Histórico por período, agrupado por semana (começando na segunda-feira) ou por mês, com número de sessões e maior carga de cada grupo.
- Evolução da carga ao longo do tempo.
- Volume semanal por grupo muscular.
- Edição e exclusão da própria conta. <!-- REVISAR: o board lista como funcionalidade de usuário; confirmar se fica no MVP ou nos extras. -->

## Tecnologias

- .NET 10 e ASP.NET Core Web API (Controllers)
- Arquitetura em camadas: Controller, Service e Repository
- Entity Framework Core com migrations
- Autenticação por token JWT
- Swagger para documentação e demonstração
- xUnit para testes automatizados
- Git e GitHub (branches, Conventional Commits, Pull Requests e GitHub Projects)

## Como executar localmente

### Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- Git
- Ferramenta do Entity Framework Core: `dotnet tool install --global dotnet-ef`

### Passo a passo

1. Clone o repositório e entre na pasta:

   ```bash
   git clone <url-do-repositorio>
   cd <pasta-do-repositorio>
   ```

2. Configure os segredos. Nenhum segredo fica no repositório: use o `user-secrets` no ambiente local.

   ```bash
   cd <pasta-do-projeto-da-api>
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "uma-chave-longa-e-aleatoria-com-32-ou-mais-caracteres"
   dotnet user-secrets set "ConnectionStrings:Default" "Data Source=treinos.db"
   ```

   <!-- REVISAR: a string de conexão acima é de SQLite; ajustar se o grupo usar SQL Server ou MySQL. -->

3. Volte para a raiz e restaure, compile e rode os testes:

   ```bash
   dotnet restore
   dotnet build
   dotnet test
   ```

4. Aplique as migrations para criar o banco:

   ```bash
   dotnet ef database update --project <pasta-do-projeto-da-api>
   ```

5. Execute a API:

   ```bash
   dotnet run --project <pasta-do-projeto-da-api>
   ```

6. Abra o Swagger no endereço exibido no terminal, normalmente `https://localhost:<porta>/swagger`.

Os testes rodam com `dotnet test` e não dependem do banco publicado.

## Testes automatizados

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

## Testar pelo Swagger

### Autenticação

1. `POST /api/auth/cadastro` com nome, e-mail e senha (mínimo de 8 caracteres): deve responder 201.
2. `POST /api/auth/login` com o mesmo e-mail e senha: copie o `token` da resposta.
3. Clique em **Authorize** (cadeado no topo), cole o token e confirme. Sem token, as rotas protegidas respondem 401.

### Exercícios (`/api/exercicios`)

1. `POST` com `{"nome": "Supino reto", "grupoMuscular": "Peito"}`: deve responder 201 com o header `Location`.
2. `POST` com `"nome": "S"` ou `"grupoMuscular": "Gluteos"`: deve responder 400, apontando o campo em `errors`. Grupos aceitos: Peito, Costas, Pernas, Ombros, Braços, Abdômen ou Outro.
3. `GET /api/exercicios`: lista só os exercícios da usuária logada.
4. `GET`, `PUT` e `DELETE` em `/api/exercicios/{id}`: devem responder 200, 200 e 204.
5. Isolamento: cadastre uma segunda usuária, faça login com ela, clique em **Authorize** com o novo token e tente acessar o exercício da primeira. Deve responder 404, como se ele não existisse.
6. Exercício que está em algum treino não pode ser excluído: o `DELETE` responde 409 pedindo para removê-lo dos treinos antes. Enquanto não houver endpoint de treinos, esse caso é verificado pelos testes automatizados.

## Diagrama das entidades

> Espaço reservado. O diagrama será adicionado na issue 3.

## Documentação da API

- Swagger publicado: _link será adicionado após o deploy_
- Tabela de rotas: _será adicionada junto com a documentação do Swagger_
