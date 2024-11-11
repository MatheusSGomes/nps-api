using Microsoft.AspNetCore.Mvc;
using NPS.Core.DTOs.Request;
using NPS.Core.DTOs.Response;
using NPS.Core.Entities;

namespace NPS.API.Controllers;

[ApiController]
[Route("nps/responses")]
public class NpsController : ControllerBase
{
    [HttpPost]
    public NpsResponseOutputDto SaveUserNpsScore(NpsRequest request)
    {
        var nps = new NpsResponse(request.Score, request.CustomerName, request.Comment, request.Category);
        return new NpsResponseOutputDto(request.Score, request.CustomerName);
    }
}
