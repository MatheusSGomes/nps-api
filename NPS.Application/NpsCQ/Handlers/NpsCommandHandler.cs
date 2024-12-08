using MediatR;
using NPS.Application.NpsCQ.ViewModels;
using NPS.Core.DTOs.Request;
using NPS.Core.Entities;
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

        // _unitOfWork.Create(nps);
        // _unitOfWork.Commit(); (esse é feito internamente)

        // Estratégias:
        // Salvo no banco (classifico na busca)
        // Classifico (salvo no banco)


        throw new NotImplementedException();

        return new NpsResponseViewModel(nps.Score, nps.CustomerName);
    }
}
