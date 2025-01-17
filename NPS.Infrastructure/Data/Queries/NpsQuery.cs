using System.Text;
using Microsoft.Extensions.Configuration;
using NPS.Core.Nps.Filters;
using NPS.Core.Nps.ViewModels;

namespace NPS.Infrastructure.Data.Queries;

public class NpsQuery : Query, INpsQuery
{
    public NpsQuery(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<int> GetNpsScore()
    {
        const string sql = @"
                SELECT
                    /* Porcentagem promotores */
                    (SELECT COUNT(Score) FROM [Nps] WHERE Score >= 9) * 100.0 /
                    (SELECT COUNT(*) FROM [Nps]) -
                    /* Porcentagem detratores*/
                    (SELECT COUNT(Score) FROM [Nps] WHERE Score <= 6) * 100.0 /
                    (SELECT COUNT(*) FROM [Nps]) AS Score;";

        return await QueryFirstAsync<int>(sql);
    }

    public async Task<IEnumerable<NpsFullResponseViewModel>> GetNpsResponses(NpsFilters filters)
    {
        StringBuilder sql = new();

        sql.AppendLine("SELECT Score, CustomerName, Comment FROM [Nps] WHERE 1=1");

        if (filters.Data != null)
            sql.AppendLine("AND CAST(createdAt AS DATE) = @Data");

        if (filters.CustomerName != null)
            sql.AppendLine("AND CustomerName = @CustomerName");

        if (filters.Category != null)
            sql.AppendLine("AND Category = @Category");

        return await QueryListAsync<NpsFullResponseViewModel>(
            sql: sql.ToString(),
            parameters: new
            {
                Data = filters.Data,
                CustomerName = filters.CustomerName,
                Category = filters.Category
            });
    }

    public async Task<NpsSummaryViewModel> GetNpsSummary()
    {
        var sql = @"SELECT 
                            /* Promoters */
                            CAST((SELECT COUNT(Score) FROM [Nps] WHERE Score >= 9) * 100.0 /
                                 (SELECT COUNT(*) FROM [Nps]) AS DECIMAL(10, 2)) AS Promoters,
                            /* Neutrals */
                            CAST((SELECT COUNT(Score) FROM [Nps] WHERE Score > 6 AND Score < 9) * 100.0 /
                                 (SELECT COUNT(*) FROM [Nps]) AS DECIMAL(10, 2)) AS Neutrals,
                            /* Detractors */
                            CAST((SELECT COUNT(Score) FROM [Nps] WHERE Score <= 6) * 100.0 /
                                 (SELECT COUNT(*) FROM [Nps]) AS DECIMAL(10, 2)) AS Detractors,
                            /* NpsScore */
                            CAST((SELECT COUNT(Score) FROM [Nps] WHERE Score >= 9) * 100.0 /
                                 (SELECT COUNT(*) FROM [Nps]) -
                                 (SELECT COUNT(Score) FROM [Nps] WHERE Score <= 6) * 100.0 /
                                 (SELECT COUNT(*) FROM [Nps]) AS Decimal(10, 2)) AS NpsScore;";

        return await QueryFirstAsync<NpsSummaryViewModel>(sql);
    }
}
