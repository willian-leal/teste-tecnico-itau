namespace InvestimentosApi.DTOs;

public class ResumoGlobalDto
{
    public int UsuarioId { get; set; }
    public decimal TotalInvestido { get; set; }
    public decimal ValorAtual { get; set; }
    public decimal LucroPrejuizo { get; set; }
}