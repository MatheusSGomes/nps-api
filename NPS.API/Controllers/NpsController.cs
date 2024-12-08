using Microsoft.AspNetCore.Mvc;
using NPS.Core.DTOs.Request;
using NPS.Core.Entities;
using MediatR;
using NPS.Application.NpsCQ.ViewModels;

namespace NPS.API.Controllers;

[ApiController]
[Route("nps/responses")]
public class NpsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NpsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<NpsResponseViewModel>> SaveUserNpsScore(NpsCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}
