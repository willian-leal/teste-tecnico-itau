namespace InvestimentosApi.Services;

public class PrecoMedioService
{
    public decimal CalcularPrecoMedio(List<(int Quantidade, decimal PrecoUnitario)> compras)
    {
        if (compras == null || compras.Count == 0)
            throw new ArgumentException("A lista de compras está vazia.");

        var somaTotal = 0m;
        var somaQuantidade = 0;

        foreach (var (quantidade, preco) in compras)
        {
            if (quantidade <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.");

            somaTotal += quantidade * preco;
            somaQuantidade += quantidade;
        }

        return somaTotal / somaQuantidade;
    }
}