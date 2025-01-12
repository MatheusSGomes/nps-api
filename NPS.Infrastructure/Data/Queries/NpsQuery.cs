using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace NPS.Infrastructure.Data.Queries;

public class NpsQuery : INpsQuery
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
}
