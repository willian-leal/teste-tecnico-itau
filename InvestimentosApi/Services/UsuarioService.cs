using InvestimentosApi.Data;
using InvestimentosApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InvestimentosApi.Services;

public class UsuarioService
{
    private readonly InvestimentosDbContext _context;
    public UsuarioService(InvestimentosDbContext context)
    {
        _context = context;
    }

    public async Task<List<PosicaoDto>> ObterPosicoesAsync(int usuarioId)
    {
        return await _context.Posicao
            .Where(p => p.UsuarioId == usuarioId)
            .Include(p => p.Ativo)
            .Select(p => new PosicaoDto
            {
                Ativo = p.Ativo!.Codigo,
                Quantidade = p.Quantidade,
                PrecoMedio = p.PrecoMedio,
                PnL = p.PnL
            })
            .ToListAsync();
    }

    public async Task<CorretagemDto> ObterCorretagemTotalAsync(int usuarioId)
    {
        var total = await _context.Operacao
            .Where(o => o.UsuarioId == usuarioId)
            .SumAsync(o => o.Corretagem);

        return new CorretagemDto
        {
            UsuarioId = usuarioId,
            TotalCorretagem = total
        };
    }

    public async Task<List<InvestimentosPorAtivoDto>> ObterTotalInvestidoPorAtivoAsync(int usuarioId)
    {
        return await _context.Operacao
            .Where(o => o.UsuarioId == usuarioId && o.TipoOperacao == "COMPRA")
            .GroupBy(o => o.Ativo!.Codigo)
            .Select(g => new InvestimentosPorAtivoDto
            {
                Ativo = g.Key,
                TotalInvestido = g.Sum(o => o.Quantidade * o.PrecoUnitario)
            })
            .ToListAsync();
    }

    public async Task<ResumoGlobalDto> ObterResumoGlobalAsync(int usuarioId)
    {
        var posicoes = await _context.Posicao
            .Where(p => p.UsuarioId == usuarioId)
            .Include(p => p.Ativo)
            .ToListAsync();

        var totalInvestido = posicoes.Sum(p => p.Quantidade * p.PrecoMedio);
        var valorAtual = posicoes.Sum(p => (p.Quantidade * (p.PrecoMedio + p.PnL / p.Quantidade)));

        return new ResumoGlobalDto
        {
            UsuarioId = usuarioId,
            TotalInvestido = totalInvestido,
            ValorAtual = valorAtual,
            LucroPrejuizo = valorAtual - totalInvestido
        };
    }

    public async Task<decimal?> ObterPrecoMedioAsync(int usuarioId, int ativoId)
    {
        var posicao = await _context.Posicao
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.AtivoId == ativoId);

        return posicao?.PrecoMedio;
    }

}