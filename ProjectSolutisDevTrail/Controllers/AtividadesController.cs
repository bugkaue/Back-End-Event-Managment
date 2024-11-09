using Microsoft.AspNetCore.Mvc;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ProjectSolutisDevTrail.Controllers;

[Route("[controller]")]
[ApiController]
public class AtividadesController : ControllerBase
{
    private readonly IAtividadeService _atividadeService;

    public AtividadesController(IAtividadeService atividadeService)
    {
        _atividadeService = atividadeService;
    }

    [Authorize(Policy = "Admin" )]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AtividadeRecente>>> GetAtividadesRecentes()
    {
        var atividades = await _atividadeService.GetAtividadesRecentesAsync();
        return Ok(atividades);
    }
}