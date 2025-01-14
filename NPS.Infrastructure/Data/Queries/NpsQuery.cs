using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NPS.Core.Nps.Filters;
using NPS.Core.Nps.ViewModels;

namespace NPS.Infrastructure.Data.Queries;

public class NpsQuery : Query, INpsQuery
{
    private readonly IConfiguration _configuration;

    public NpsQuery(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<int> GetNpsScore()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        await using (var connection = new SqlConnection(connectionString))
        {
            var sql = @"
                SELECT
                    /* Porcentagem promotores */
                    (SELECT COUNT(Score) FROM [Nps] WHERE Score >= 9) * 100.0 /
                    (SELECT COUNT(*) FROM [Nps]) -
                    /* Porcentagem detratores*/
                    (SELECT COUNT(Score) FROM [Nps] WHERE Score <= 6) * 100.0 /
                    (SELECT COUNT(*) FROM [Nps]) AS Score;";

            return await connection.QueryFirstOrDefaultAsync<int>(sql);
        }
    }

    public async Task<IEnumerable<NpsFullResponseViewModel>> GetNpsResponses(NpsFilters filters)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        await using (var connection = new SqlConnection(connectionString))
        {
            StringBuilder sql = new();

            sql.AppendLine("SELECT Score, CustomerName, Comment FROM [Nps]");

            if (filters.Data != null)
                sql.AppendLine("WHERE CAST(createdAt AS DATE) = @Data;");

            return await connection.QueryAsync<NpsFullResponseViewModel>(
                sql: sql.ToString(),
                param: new { Data = filters.Data });
        }
    }
}
