using InvestimentosApi.Data;
using InvestimentosApi.DTOs;
using InvestimentosApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestimentosApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CotacoesController : ControllerBase
{
    private readonly InvestimentosDbContext _context;
    private readonly ILogger<CotacoesController> _logger;

    public CotacoesController(InvestimentosDbContext context, ILogger<CotacoesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> PostCotacao([FromBody] CotacaoDto dto)
    {
        _logger.LogInformation($"Recebido: AtivoId={dto.AtivoId}, PrecoUnitario={dto.PrecoUnitario}, DataHora={dto.DataHora}");

        var cotacaoExiste = await _context.Cotacao.AnyAsync(c =>
            c.AtivoId == dto.AtivoId &&
            c.DataHora == dto.DataHora
        );

        if (cotacaoExiste)
        {
            return Conflict("Cotação já existente para este Ativo e DataHora.");
        }

        var cotacao = new Cotacao
        {
            AtivoId = dto.AtivoId,
            PrecoUnitario = dto.PrecoUnitario,
            DataHora = dto.DataHora
        };

        _context.Cotacao.Add(cotacao);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
