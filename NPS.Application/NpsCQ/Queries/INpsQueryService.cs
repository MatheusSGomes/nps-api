using NPS.Core.Nps.Filters;
using NPS.Core.Nps.ViewModels;

namespace NPS.Application.NpsCQ.Queries;

public interface INpsQueryService
{
    Task<NpsScoreViewModel> GetNpsScore();
    Task<IEnumerable<NpsFullResponseViewModel>> GetNpsResponses(NpsFilters filters);
    Task<NpsSummaryViewModel> GetNpsSummary();
}
