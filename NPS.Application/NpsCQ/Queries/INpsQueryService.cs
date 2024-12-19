using NPS.Application.NpsCQ.ViewModels;

namespace NPS.Application.NpsCQ.Queries;

public interface INpsQueryService
{
    Task<NpsScoreViewModel> GetNpsScore();
}
