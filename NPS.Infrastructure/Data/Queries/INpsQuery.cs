using NPS.Core.Nps.Filters;
using NPS.Core.Nps.ViewModels;

namespace NPS.Infrastructure.Data.Queries;

public interface INpsQuery
{
    Task<string> GetNpsScore();
    Task<IEnumerable<NpsFullResponseViewModel>> GetNpsResponses(NpsFilters filters);
    Task<NpsSummaryViewModel> GetNpsSummary();
}
