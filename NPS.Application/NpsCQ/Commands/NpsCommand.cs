using MediatR;
using NPS.Core.Nps.ViewModels;

namespace NPS.Core.DTOs.Request;

public class NpsCommand : IRequest<NpsResponseViewModel>
{
    public int Score { get; set; }
    public string CustomerName { get; set; }
    public string Comment { get; set; }
}
