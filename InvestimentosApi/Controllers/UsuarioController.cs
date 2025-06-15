using InvestimentosApi.Models;
using InvestimentosApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestimentosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly UsuarioService _usuarioService;
    public UsuarioController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }
    [HttpGet("{id}/posicao")]
    public async Task<IActionResult> ObterPosicoes(int id)
    {
        var posicoes = await _usuarioService.ObterPosicoesAsync(id);
        return Ok(posicoes);
    }

    [HttpGet("{id}/corretagem")]
    public async Task<IActionResult> ObterCorretagemTotal(int id)
    {
        var corretagem = await _usuarioService.ObterCorretagemTotalAsync(id);
        return Ok(corretagem);
    }

    [HttpGet("{id}/investido")]
    public async Task<IActionResult> ObterTotalInvestidoPorAtivo(int id)
    {
        var investimentos = await _usuarioService.ObterTotalInvestidoPorAtivoAsync(id);
        return Ok(investimentos);
    }

    [HttpGet("{id}/resumo")]
    public async Task<IActionResult> ObterResumoGlobal(int id)
    {
        var resumo = await _usuarioService.ObterResumoGlobalAsync(id);
        return Ok(resumo);
    }
}