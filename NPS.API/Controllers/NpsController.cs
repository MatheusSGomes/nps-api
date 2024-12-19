using MediatR;
using Microsoft.AspNetCore.Mvc;
using NPS.Application.NpsCQ.Queries;
using NPS.Application.NpsCQ.ViewModels;
using NPS.Core.DTOs.Request;

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
    public async Task<ActionResult<NpsScoreViewModel>> GetNpsScore()
    {
        return Ok(await _npsQueryService.GetNpsScore());
    }
}
