
# Título do Projeto

Uma breve descrição sobre o que esse projeto faz e para quem ele é

# Teste Técnico - Itaú Unibanco (Fase 2)

Este repositório contém a resolução da segunda fase do processo seletivo do Itaú Unibanco, com foco em um sistema de controle de investimentos em renda variável.

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

### Autorreferência solicitada

> Gosto de ser chamado de Camarão  
> Resposta : 01000101 01110101 00100000 01100001 01101101 01101111 00100000 01100011 01100001 01101101 01100001 01110010 11000011 10100000 01101111

## Parte 2 – Índices e Performance

### Objetivo

O sistema precisa realizar duas ações com alto desempenho:

1. Consultar rapidamente todas as operações de um usuário em um determinado ativo nos últimos 30 dias.
2. Atualizar em tempo real o campo `pnl` (lucro/prejuízo) da posição de cada cliente ao inserir uma nova cotação.

---

### Índices criados

Para garantir performance nas consultas e atualizações, foram criados os seguintes índices:

- `idx_oper_usuario_ativo_data` em `operacoes(usuario_id, ativo_id, data_hora)`
- `idx_cot_ativo_data` em `cotacao(ativo_id, data_hora DESC)`
- `idx_pos_usuario_ativo` em `posicao(usuario_id, ativo_id)`

---

### Consulta SQL otimizada

   A seguinte query retorna as operações recentes com base nos filtros exigidos:

   `sql
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
   ORDER BY data_hora DESC;`

### Atualização de P&L (lucro/prejuízo)

Uma `procedure` e uma `trigger` foram criadas no script `init.sql` para atualizar automaticamente o campo `pnl` da tabela `posicao` sempre que uma nova cotação for inserida.

#### Procedure

`sql
CALL sp_update_posicao(ativo_id, preco_mercado);`

#### Trigger automática
`CREATE TRIGGER tr_cotacao_ai
AFTER INSERT ON cotacao
FOR EACH ROW
CALL sp_update_posicao(NEW.ativo_id, NEW.preco_unitario);`

Com isso, a atualização do pnl acontece de forma automática, garantindo consistência e resposta em tempo real sempre que a tabela cotacao for atualizada.