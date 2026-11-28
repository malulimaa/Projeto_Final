# Rotas da API

Planejamento das rotas antes da implementação. Cada rota lista o método, o caminho, o que faz, se exige token e os códigos de resposta esperados.

Status: ✅ implementada · 📝 planejada · ❓ proposta, precisa de aprovação do grupo

## Autenticação

| Método | Rota | Descrição | Token | Respostas | Status |
|---|---|---|---|---|---|
| POST | `/api/auth/cadastro` | Cria a conta de uma nova usuária (nome, e-mail, senha com no mínimo 8 caracteres) | Não | 201 conta criada · 400 dados inválidos · 409 e-mail já cadastrado | ✅ |
| POST | `/api/auth/login` | Autentica a usuária e devolve o token | Não | 200 token gerado · 400 dados inválidos · 401 e-mail ou senha inválidos | ✅ |
| GET | `/api/auth/eu` | Rota de teste: devolve o Id da usuária logada | Sim | 200 Id da usuária · 401 sem token ou token inválido | ✅ |

## Exercícios

| Método | Rota | Descrição | Token | Respostas | Status |
|---|---|---|---|---|---|
| GET | `/api/exercicios` | Lista os exercícios da usuária logada | Sim | 200 lista (pode ser vazia) · 401 sem token | ✅ |
| GET | `/api/exercicios/{id}` | Busca um exercício da usuária pelo Id | Sim | 200 exercício · 401 sem token · 404 não existe ou é de outra usuária | ✅ |
| POST | `/api/exercicios` | Cadastra um exercício (nome de 2 a 60 caracteres, descrição opcional, grupo muscular) | Sim | 201 criado, com header `Location` · 400 dados inválidos · 401 sem token | ✅ |
| PUT | `/api/exercicios/{id}` | Altera um exercício da usuária | Sim | 200 alterado · 400 dados inválidos · 401 sem token · 404 não existe ou é de outra usuária | ✅ |
| DELETE | `/api/exercicios/{id}` | Exclui um exercício da usuária | Sim | 204 excluído · 401 sem token · 404 não existe ou é de outra usuária · 409 exercício está em algum treino | ✅ |

Grupos musculares aceitos: Peito, Costas, Pernas, Ombros, Braços, Abdômen ou Outro.

## Treinos

| Método | Rota | Descrição | Token | Respostas | Status |
|---|---|---|---|---|---|
| GET | `/api/treinos` | Lista os treinos da usuária logada | Sim | 200 lista (pode ser vazia) · 401 sem token | 📝 |
| GET | `/api/treinos/{id}` | Busca um treino da usuária pelo Id, com as séries | Sim | 200 treino · 401 sem token · 404 não existe ou é de outra usuária | 📝 |
| POST | `/api/treinos` | Cria um treino com nome, observações e séries | Sim | 201 criado, com header `Location` · 400 dados inválidos · 401 sem token · 404 exercício de alguma série não existe ou é de outra usuária | 📝 |
| PUT | `/api/treinos/{id}` | Altera o treino e as séries do plano | Sim | 200 alterado · 400 dados inválidos · 401 sem token · 404 treino ou exercício de alguma série não existe ou é de outra usuária | 📝 |
| DELETE | `/api/treinos/{id}` | Exclui o treino e as séries do plano. As sessões já registradas são mantidas | Sim | 204 excluído · 401 sem token · 404 não existe ou é de outra usuária | 📝 |

Cada série tem exercício, ordem, repetições alvo e carga sugerida. O 400 cobre:

- treino sem nenhuma série;
- ordem repetida dentro do mesmo treino (400, e não 409);
- carga sugerida negativa (zero é válido);
- repetições alvo fora de 1 a 100.

Editar o treino não altera o que já foi registrado nas sessões.

## Sessões

| Método | Rota | Descrição | Token | Respostas | Status |
|---|---|---|---|---|---|
| POST | `/api/sessoes` | Inicia uma sessão a partir de um treino da usuária. Pode já trazer as séries executadas (carga e repetições reais) ou vir sem nenhuma | Sim | 201 criada, com header `Location` · 400 dados inválidos ou série que não é do treino · 401 sem token · 404 treino não existe ou é de outra usuária | 📝 |
| GET | `/api/sessoes` | Lista as sessões da usuária logada | Sim | 200 lista (pode ser vazia) · 401 sem token | 📝 |
| GET | `/api/sessoes/{id}` | Busca uma sessão da usuária, com as séries registradas e as não preenchidas | Sim | 200 sessão · 401 sem token · 404 não existe ou é de outra usuária | ❓ |
| POST | `/api/sessoes/{id}/series` | Registra uma ou mais séries em uma sessão já iniciada (registro aos poucos) | Sim | 201 séries registradas · 400 dados inválidos ou série que não é do treino da sessão · 401 sem token · 404 sessão não existe ou é de outra usuária | ❓ |

O 400 cobre:

- série que não pertence ao treino da sessão;
- carga negativa (zero é válido, para exercícios sem peso);
- repetições fora de 1 a 100.

Série não preenchida fica como não executada: não entra na maior carga nem no histórico.

O registro guarda a carga e as repetições realmente feitas, separadas do plano. Se o treino mudar depois (por exemplo, a carga sugerida passar de 40 kg para 50 kg), a sessão continua mostrando 40 kg.
