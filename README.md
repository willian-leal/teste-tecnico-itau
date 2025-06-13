
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
