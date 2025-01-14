using MediatR;
using NPS.Core.DTOs.Request;
using NPS.Core.Entities;
using NPS.Core.Nps.ViewModels;
using NPS.Infrastructure.UnitOfWork;

namespace NPS.Application.NpsCQ.Handlers;

public class NpsCommandHandler : IRequestHandler<NpsCommand, NpsResponseViewModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public NpsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<NpsResponseViewModel> Handle(NpsCommand request, CancellationToken cancellationToken)
    {
        var nps = Nps.CreateResponse(request.Score, request.CustomerName, request.Comment);
        _unitOfWork.NpsRepository.Create(nps);
        return new NpsResponseViewModel(nps.Score, nps.CustomerName);
    }
}
