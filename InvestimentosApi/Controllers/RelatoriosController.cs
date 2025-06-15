using InvestimentosApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RelatoriosController : ControllerBase
{
    private readonly InvestimentosDbContext _context;

    public RelatoriosController(InvestimentosDbContext context)
    {
        _context = context;
    }

    [HttpGet("top-posicoes")]
    public async Task<IActionResult> TopClientesPorPosicao()
    {
        var resultado = await _context.Posicao
            .GroupBy(p => p.UsuarioId)
            .Select(g => new {
                UsuarioId = g.Key,
                TotalInvestido = g.Sum(p => p.Quantidade * p.PrecoMedio)
            })
            .OrderByDescending(x => x.TotalInvestido)
            .Take(10)
            .ToListAsync();

        return Ok(resultado);
    }

    [HttpGet("top-corretagens")]
    public async Task<IActionResult> TopClientesPorCorretagem()
    {
        var resultado = await _context.Operacao
            .GroupBy(o => o.UsuarioId)
            .Select(g => new {
                UsuarioId = g.Key,
                TotalCorretagem = g.Sum(o => o.Corretagem)
            })
            .OrderByDescending(x => x.TotalCorretagem)
            .Take(10)
            .ToListAsync();

        return Ok(resultado);
    }
}
