using Microsoft.AspNetCore.Mvc;

namespace NPS.Application;

public class ErrorResponse : ProblemDetails
{
    public Dictionary<string, List<string>> Errors { get; set; }
    public string TraceId { get; set; }
}
