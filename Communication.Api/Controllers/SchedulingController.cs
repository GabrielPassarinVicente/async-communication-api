using Communication.Application.DTOs;
using Communication.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace Communication.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulingController : Controller
{
    private readonly ISchedulingService _schedulingService;

    // Recebemos o serviço que criamos no passo anterior
    public SchedulingController(ISchedulingService schedulingService)
    {
        _schedulingService = schedulingService;
    }

    [HttpPost]
    public async Task<IActionResult> Schedule([FromBody] ScheduleRequestDto request)
    {
        // Chama o serviço que orquestra tudo e pega o ID gerado
        var id = await _schedulingService.ScheduleAsync(request);

        // Retorna HTTP 201 (Created), o ID no corpo da resposta, 
        // e aponta para a rota "GetStatus"
        return CreatedAtAction(nameof(GetStatus), new { id = id}, new {Id = id});  

    }

    // 2. Consulta do envio da comunicação (GET)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        var scheduling = await _schedulingService.GetStatusAsync(id);

        if(scheduling == null)
            return NotFound(new { Message = "Agendamento não encontrado." });

        return Ok(new { Scheduling = scheduling });
    }

    // 3. Cancelamento do envio da comunicação (PATCH)
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _schedulingService.CancelAsync(id);
        // Retorna 204 No Content, que é o padrão para atualizações bem-sucedidas 
        // que não precisam de devolver dados no corpo da resposta
        return NoContent();
    }
}

