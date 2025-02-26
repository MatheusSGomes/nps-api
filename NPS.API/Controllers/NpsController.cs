using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPS.Application.NpsCQ.Queries;
using NPS.Core.DTOs.Request;
using NPS.Core.Nps.Filters;
using NPS.Core.Nps.ViewModels;

namespace NPS.API.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class NpsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly INpsQueryService _npsQueryService;

    public NpsController(IMediator mediator, INpsQueryService npsQueryService)
    {
        _mediator = mediator;
        _npsQueryService = npsQueryService;
    }

    [HttpPost("Responses")]
    public async Task<ActionResult<NpsResponseViewModel>> SaveUserNpsScore(NpsCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("Score")]
    [Authorize]
    public async Task<ActionResult<NpsScoreViewModel>> GetNpsScore()
    {
        // TODO: Adicionar cache distribuído no retorno com o tempo limite de 3 minutos
        return Ok(await _npsQueryService.GetNpsScore());
    }

    [HttpGet("Responses")]
    [Authorize]
    public async Task<ActionResult<List<NpsFullResponseViewModel>>> GetNpsResponses([FromQuery]NpsFilters filters)
    {
        return Ok(await _npsQueryService.GetNpsResponses(filters));
    }

    [HttpGet("Summary")]
    [Authorize]
    public async Task<ActionResult<NpsSummaryViewModel>> GetNpsSummary()
    {
        return Ok(await _npsQueryService.GetNpsSummary());
    }
}
