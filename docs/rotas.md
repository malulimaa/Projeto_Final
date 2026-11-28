# Rotas da API

Planejamento das rotas antes da implementação. Cada rota lista o método, o caminho, o que faz, se exige token e os códigos de resposta esperados.

Status: ✅ implementada · 📝 planejada

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

