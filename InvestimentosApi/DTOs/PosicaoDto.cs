namespace InvestimentosApi.DTOs;

public class PosicaoDto
{
    public string Ativo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoMedio { get; set; }
    public decimal PnL { get; set; }
}