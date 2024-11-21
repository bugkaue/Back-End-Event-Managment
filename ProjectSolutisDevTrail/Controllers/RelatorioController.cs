using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Controllers;

[ApiController]
[Route("[controller]")]
public class RelatorioController : ControllerBase
{
    private readonly IInscricaoService _inscricaoService;

    public RelatorioController(IInscricaoService inscricaoService)
    {
        _inscricaoService = inscricaoService;
    }

    [Authorize(Roles = "Admin" )]
    [HttpGet("gerar/{eventoId}")]
    public async Task<IActionResult> GenerateReport(int eventoId)
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"Relatorio_Evento_{eventoId}.pdf"); // Diretório temporário

        await _inscricaoService.GenerateReportAsync(eventoId, filePath);

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        var fileName = Path.GetFileName(filePath);
        var result = File(fileBytes, "application/pdf", fileName);

        System.IO.File.Delete(filePath);

        return result;
    }
}
