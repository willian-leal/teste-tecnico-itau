using Xunit;
using InvestimentosApi.Services;

namespace InvestimentosApi.Tests;

public class PrecoMedioServiceTests
{
    private readonly PrecoMedioService _service;

    public PrecoMedioServiceTests()
    {
        _service = new PrecoMedioService();
    }

    [Fact]
    public void Deve_Calcular_PrecoMedio_Corretamente()
    {
        var compras = new List<(int, decimal)>
        {
            (10, 5.00m),
            (20, 7.00m)
        };

        var resultado = _service.CalcularPrecoMedio(compras);

        Assert.Equal(6.33m, Math.Round(resultado, 2));
    }

    [Fact]
    public void Deve_Lancar_Excecao_Se_Lista_Estiver_Vazia()
    {
        var compras = new List<(int, decimal)>();

        var ex = Assert.Throws<ArgumentException>(() => _service.CalcularPrecoMedio(compras));
        Assert.Contains("vazia", ex.Message);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Se_Quantidade_For_Zero()
    {
        var compras = new List<(int, decimal)>
        {
            (0, 10.00m)
        };

        var ex = Assert.Throws<ArgumentException>(() => _service.CalcularPrecoMedio(compras));
        Assert.Contains("maior que zero", ex.Message);
    }
}
