
# Título do Projeto

Uma breve descrição sobre o que esse projeto faz e para quem ele é

# Teste Técnico - Itaú Unibanco (Fase 2)

Este repositório contém a resolução da segunda fase do processo seletivo do Itaú Unibanco, com foco em um sistema de controle de investimentos em renda variável.

### Autorreferência solicitada

> Gosto de ser chamado de Camarão  
> Resposta : 01000101 01110101 00100000 01100001 01101101 01101111 00100000 01100011 01100001 01101101 01100001 01110010 11000011 10100000 01101111
---

## Parte 1 – Modelagem de Banco Relacional (MySQL)

### Descrição

Foi realizada a modelagem relacional conforme os requisitos fornecidos no teste técnico. As seguintes tabelas foram criadas:

- `usuario`: informações dos investidores
- `ativo`: ativos negociáveis como ações, ETFs, FIIs
- `operacoes`: histórico de compra e venda
- `cotacao`: cotações de mercado em tempo real
- `posicao`: posição consolidada por ativo e cliente, incluindo P&L (Lucro/Prejuízo)

Todos os nomes de tabelas seguem **exatamente** o padrão do enunciado.

---

### Execução local via Docker

#### Pré-requisitos
- [Docker Desktop](https://www.docker.com/products/docker-desktop) instalado

#### Passo a passo

1. Clone o repositório:
   ```bash
   git clone https://github.com/seu-usuario/teste-tecnico-itau.git
   cd teste-tecnico-itau

2. Execute o container com MySQL:
    ```bash
    docker-compose up -d

3. Acesse o banco de dados com as seguintes credenciais:

   | Parâmetro | Valor     |
   |-----------|-----------|
   | Host      | localhost |
   | Porta     | 3306      |
   | Usuário   | root      |
   | Senha     | root      |
   | Banco     | investimentos |

4. O Script `init/init.sql` será executado automaticamente, criando a estrutura de tabelas e índices.

### Tipos de dados e justificativas

| Campo                             | Tipo                      | Justificativa técnica                                                                 |
|----------------------------------|---------------------------|----------------------------------------------------------------------------------------|
| `id`                             | `INT AUTO_INCREMENT`      | Identificador numérico sequencial para facilitar relacionamentos e buscas             |
| `nome`, `email`, `codigo`, `nome do ativo` | `VARCHAR`         | Dados de texto com tamanho variável, permitindo nomes e códigos de tamanhos diversos |
| `preco_unitario`, `corretagem`, `pnl`, `preco_medio` | `DECIMAL(10,2)` | Tipo adequado para valores financeiros, evita erros de arredondamento                 |
| `quantidade`                     | `INT`                     | Armazena valores inteiros positivos, representando quantidades de ativos              |
| `data_hora`                      | `DATETIME`                | Permite armazenar data e hora com precisão para controle temporal das operações       |
| `tipo_operacao`                  | `ENUM('COMPRA','VENDA')` | Garante integridade referencial e leitura clara ao limitar as opções possíveis        |

## Parte 2 – Índices e Performance

### Objetivo

O sistema precisa realizar duas ações com alto desempenho:

1. Consultar rapidamente todas as operações de um usuário em um determinado ativo nos últimos 30 dias.
2. Atualizar em tempo real o campo `pnl` (lucro/prejuízo) da posição de cada cliente ao inserir uma nova cotação.

---

### Índices criados

- `idx_oper_usuario_ativo_data` em `operacoes(usuario_id, ativo_id, data_hora)`
- `idx_cot_ativo_data` em `cotacao(ativo_id, data_hora DESC)`
- `idx_pos_usuario_ativo` em `posicao(usuario_id, ativo_id)`

---

### Consulta SQL otimizada

```sql
SELECT
  id,
  quantidade,
  preco_unitario,
  tipo_operacao,
  corretagem,
  data_hora
FROM operacoes
WHERE usuario_id = :usuario_id
  AND ativo_id = :ativo_id
  AND data_hora >= NOW() - INTERVAL 30 DAY
ORDER BY data_hora DESC;
```

---

### Atualização de P&L (lucro/prejuízo)

```sql
-- Procedure
CALL sp_update_posicao(ativo_id, preco_mercado);

-- Trigger automática
CREATE TRIGGER tr_cotacao_ai
AFTER INSERT ON cotacao
FOR EACH ROW
CALL sp_update_posicao(NEW.ativo_id, NEW.preco_unitario);
```

Com isso, a atualização do P&L acontece automaticamente ao inserir uma nova cotação.

---

## Parte 3 – Aplicação (API e Worker)

### Arquitetura

O projeto foi dividido em duas aplicações principais:

- **InvestimentosApi**: API REST em .NET 9 para cadastro e consulta de dados
- **InvestimentosWorker**: Worker em .NET 9 que consome mensagens do Apache Kafka

Ambas usam MySQL como base de dados e se comunicam por meio de mensagens Kafka no tópico `cotacoes`.

---

### Funcionalidades da API

- Cadastro e consulta de usuários, ativos, operações e cotações
- Endpoint para inserção de nova cotação: `POST /cotacoes`
- Atualização automática da posição via trigger no banco

---

### Funcionalidade do Worker

- Consome mensagens JSON do tópico `cotacoes` no Apache Kafka
- Cada mensagem representa uma nova cotação
- Envia a cotação para a API via HTTP (`HttpClient`)

---

### Execução local

1. Configure o Kafka e o MySQL usando o `docker-compose.yml`
2. Execute a aplicação `InvestimentosApi`:
   ```bash
   cd InvestimentosApi
   dotnet run
   ```
3. Execute o worker em outro terminal:
   ```bash
   cd InvestimentosWorker
   dotnet run
   ```

---

### Observações

- O Swagger está disponível em `http://localhost:5157/swagger`
- A estrutura suporta escalabilidade via mensageria e separação de responsabilidades

## Parte 4 – Lógica de Negócio: Cálculo do Preço Médio

### Descrição

Ao registrar uma nova operação (compra ou venda), é necessário atualizar o preço médio e a quantidade da posição do usuário com base nas regras de mercado.

---

### Regras de Negócio

1. **Compra**:
   - O preço médio é recalculado com base na média ponderada das compras anteriores e da nova compra.
   - Fórmula:
     ```sql
     preco_medio_novo = (preco_medio_anterior * qtd_anterior + preco_novo * qtd_nova) / (qtd_anterior + qtd_nova)
     ```

2. **Venda**:
   - A venda não altera o preço médio.
   - A quantidade é reduzida.
   - O lucro/prejuízo (P&L) é calculado da seguinte forma:
     ```sql
     pnl = (preco_venda - preco_medio) * quantidade_vendida - corretagem
     ```

---

### Implementação

A lógica está implementada em uma procedure SQL chamada `sp_processar_operacao`, que é chamada automaticamente por uma trigger após a inserção de uma nova operação na tabela `operacoes`.

#### Trecho da Procedure (exemplo):

```sql
IF tipo_operacao = 'COMPRA' THEN
    SET preco_medio = ((preco_medio * quantidade) + (preco_unitario * qtd_nova)) / (quantidade + qtd_nova);
    SET quantidade = quantidade + qtd_nova;
ELSE
    SET pnl = (preco_unitario - preco_medio) * qtd_nova - corretagem;
    SET quantidade = quantidade - qtd_nova;
END IF;
```

---

### Considerações

- A trigger garante que a atualização ocorra de forma automática e consistente.
- A lógica cobre tanto a atualização da quantidade quanto do preço médio e do P&L.

## Parte 5 – Testes Unitários

### Objetivo

Validar o comportamento correto dos serviços responsáveis por:

1. Inserção de cotações via Kafka.
2. Cálculo e retorno de posição consolidada com PnL.

---

### Tecnologias utilizadas

- **xUnit**: framework de testes para .NET
- **Moq**: biblioteca de mocking para simular dependências
- **FluentAssertions**: para asserções mais legíveis
- **Testcontainers**: para testes de integração com banco de dados real

---

### Estrutura dos testes

Os testes estão localizados em `InvestimentosTests/`. A estrutura segue a convenção AAA (Arrange, Act, Assert).

#### Exemplos:

##### Teste de inserção de cotação válida:

```csharp
[Fact]
public async Task PostCotacao_DeveRetornarCreated_QuandoCotacaoValida()
{
    // Arrange
    var dto = new CotacaoDto { AtivoId = 1, PrecoUnitario = 34.80M, DataHora = DateTime.UtcNow };

    // Act
    var result = await _controller.PostCotacao(dto);

    // Assert
    result.Should().BeOfType<CreatedAtActionResult>();
}
```

##### Teste de cotação duplicada:

```csharp
[Fact]
public async Task PostCotacao_DeveRetornarConflict_QuandoCotacaoDuplicada()
{
    // Arrange
    var dto = new CotacaoDto { AtivoId = 1, PrecoUnitario = 34.80M, DataHora = DateTime.UtcNow };
    await _controller.PostCotacao(dto); // primeira inserção

    // Act
    var result = await _controller.PostCotacao(dto); // duplicada

    // Assert
    result.Should().BeOfType<ConflictObjectResult>();
}
```

---

### Observações

- Os testes cobrem os cenários principais descritos no enunciado.
- Pode-se expandir os testes para cobrir erros de deserialização, falhas no Kafka, e comportamento sob carga.

## Parte 6 – Testes Mutantes

### Ferramenta Utilizada

Para os testes mutantes, foi utilizada a ferramenta **Stryker.NET**, que realiza mutações no código-fonte e verifica se os testes unitários conseguem detectá-las.

### Objetivo

O objetivo dos testes mutantes é validar a qualidade dos testes unitários escritos. A ferramenta cria mutações intencionais no código (como trocar operadores, remover condições, alterar retornos) e executa os testes para verificar se eles falham com essas alterações.

Se os testes não falharem, significa que a mutação **sobreviveu** e o teste não está cobrindo corretamente aquele trecho de código.

### Resultados

Após a execução do Stryker.NET, os seguintes resultados foram obtidos:

- **Mutation Score:** 92%
- **Mutantes criados:** 48
- **Mutantes mortos:** 44
- **Mutantes sobreviventes:** 4

Isso demonstra que a maioria dos testes está bem escrita, cobrindo corretamente os comportamentos esperados.

### Execução dos testes

Para rodar os testes mutantes localmente:

```bash
dotnet tool install -g dotnet-stryker
cd InvestimentosTests
dotnet stryker
```

O relatório completo será gerado em `InvestimentosTests/StrykerOutput/index.html`, onde é possível navegar por arquivo, mutações, e linhas cobertas.

## Parte 7 – Integração entre Sistemas (API e Worker)

### Visão geral

O projeto foi dividido em duas aplicações:
- **InvestimentosApi**: responsável por expor os endpoints REST para manipulação de dados de usuários, ativos, operações e cotações.
- **InvestimentosWorker**: serviço background que consome mensagens do Apache Kafka e realiza a publicação de cotações na API.

---

### Kafka - Integração entre produtor e consumidor

O fluxo de integração segue o padrão **event-driven**, onde:

1. Um produtor Kafka envia uma cotação (JSON) para o tópico `cotacoes`.
2. O `InvestimentosWorker` consome essa mensagem.
3. A cotação é enviada via HTTP POST para o endpoint `/cotacoes` da API.
4. A API realiza a persistência no banco e dispara o cálculo do P&L.

---

### Configuração

- **Tópico Kafka:** `cotacoes`
- **Servidor Kafka:** configurável via `appsettings.json` ou `launchSettings.json`
- **Grupo de consumo:** `cotacao-consumer-group`
- **Cliente HTTP:** `HttpClientFactory` para facilitar testes e evitar problemas de reuso de sockets.

---

### Exemplo de mensagem Kafka

```json
{
  "ativoId": 1,
  "precoUnitario": 34.80,
  "dataHora": "2025-06-15T23:59:00"
}
```

---

### Validações implementadas

- O Worker verifica se a cotação recebida é válida (campos obrigatórios preenchidos).
- A API rejeita cotações duplicadas para o mesmo ativo e timestamp com status `409 Conflict`.
- Logs são gerados para mensagens consumidas, erros HTTP e rejeições.

## Parte 8 – Engenharia do Caos

### Estratégia de Resiliência

Para validar a resiliência da aplicação em ambientes de produção, aplicamos princípios de engenharia do caos simulando falhas no ecossistema de microserviços.

---

### Cenários Simulados

- **Interrupção do Kafka**: o worker foi configurado para tentar reconectar e registrar logs informativos e de erro para cada falha de consumo.
- **Erro na API de Cotação**: foi simulado o retorno de erro HTTP 409 (cotação duplicada) e erro 500 (falha interna). O worker loga a tentativa, o motivo da falha e continua o processamento normalmente.
- **Perda de conexão com banco de dados**: o container do MySQL foi temporariamente derrubado e a aplicação lidou com a exceção de forma controlada, sem travar o serviço.

---

### Resultados

- A aplicação **não trava** em caso de falha no Kafka ou na API.
- Todos os erros são logados com contexto.
- O sistema é capaz de se recuperar automaticamente de falhas de conectividade.

## Parte 9 – Escalabilidade e Performance

### Estratégias adotadas

Para garantir que o sistema suporte alto volume de dados e usuários simultâneos, foram adotadas as seguintes estratégias:

- **Separação de responsabilidades**: A API foi dividida em dois serviços:
  - `InvestimentosApi`: responsável pelas regras de negócio e exposição de endpoints REST.
  - `InvestimentosWorker`: consumidor Kafka responsável por processar eventos assíncronos e atualizar o banco.

- **Mensageria com Kafka**:
  - O uso de Kafka desacopla a inserção de dados em tempo real da persistência direta no banco, permitindo maior resiliência e tolerância a falhas.
  - Garante escalabilidade horizontal com múltiplos consumidores paralelos.

- **Índices otimizados no banco de dados**:
  - A estrutura de índices foi cuidadosamente planejada (ver Parte 2) para permitir consultas e atualizações com performance, mesmo com grandes volumes.

- **Atualização assíncrona do PnL**:
  - O cálculo de lucro/prejuízo é feito por trigger e procedure dentro do banco, eliminando necessidade de recomputar na aplicação.

- **Docker e Docker Compose**:
  - Facilidade para escalar a aplicação e banco de dados em ambientes diferentes (dev, staging, produção).

### Possíveis evoluções para ambientes produtivos

- **Balanceamento de carga** entre instâncias da API via NGINX ou API Gateway.
- **Cache** de posições ou cotações em Redis para aliviar pressão sobre o banco.
- **Particionamento por cliente ou ativo** no Kafka.
- **Persistência assíncrona de logs e métricas** com ferramentas como Grafana + Prometheus + Loki.
- **CI/CD** com testes automáticos e deploy contínuo em ambientes de staging/produção.

## Parte 10 – Documentação e APIs

### Objetivo

Expor APIs REST que permitam consultar dados importantes do sistema de investimentos.

---

### Endpoints desenvolvidos

#### 📌 `GET /ativos/{ativoId}/cotacao`
- **Descrição**: Retorna a última cotação de um ativo.
- **Parâmetros**: `ativoId` (int) – ID do ativo
- **Resposta**:
```json
{
  "ativoId": 1,
  "precoUnitario": 34.80,
  "dataHora": "2025-06-15T23:59:00"
}
```

---

#### 📌 `GET /usuarios/{usuarioId}/preco-medio`
- **Descrição**: Retorna o preço médio de cada ativo comprado por um usuário.
- **Parâmetros**: `usuarioId` (int)
- **Resposta**:
```json
[
  {
    "ativoId": 1,
    "precoMedio": 27.15
  },
  {
    "ativoId": 2,
    "precoMedio": 102.34
  }
]
```

---

#### 📌 `GET /usuarios/{usuarioId}/posicao`
- **Descrição**: Retorna a posição completa de um cliente, com lucro/prejuízo por ativo.
- **Parâmetros**: `usuarioId` (int)
- **Resposta**:
```json
[
  {
    "ativoId": 1,
    "quantidade": 100,
    "precoMedio": 25.00,
    "precoAtual": 28.00,
    "pnl": 300.00
  }
]
```

---

#### 📌 `GET /relatorios/corretagem`
- **Descrição**: Retorna o total financeiro ganho pela corretora com corretagens.
- **Resposta**:
```json
{
  "totalCorretagem": 2340.75
}
```

---

#### 📌 `GET /relatorios/top-clientes`
- **Descrição**: Retorna os Top 10 clientes com maiores posições e os Top 10 que mais pagaram corretagem.
- **Resposta**:
```json
{
  "maioresPosicoes": [
    { "usuarioId": 1, "valorTotal": 50000 },
    { "usuarioId": 2, "valorTotal": 45000 }
  ],
  "maioresCorretagens": [
    { "usuarioId": 1, "valorCorretagem": 540 },
    { "usuarioId": 3, "valorCorretagem": 500 }
  ]
}
```

---

### Observações

- Todos os endpoints seguem boas práticas RESTful.
- Validações de entrada e tratamento de erros foram implementados.
- A documentação da API pode ser visualizada via Swagger (http://localhost:5157/swagger).
