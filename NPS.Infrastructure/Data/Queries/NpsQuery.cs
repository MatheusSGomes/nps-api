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
                    /* Porcentagem dos promotores */
                    COALESCE(
                            (SELECT AVG(Score) FROM [Nps] WHERE Score > 9) *
                            (SELECT COUNT(*) FROM [Nps] WHERE Score > 9) / (SELECT COUNT(*) FROM [Nps]),
                            0
                    ) -
                    /* Porcentagem dos detratores */
                    COALESCE(
                            (SELECT AVG(Score) FROM [Nps] WHERE Score < 6) *
                            (SELECT COUNT(*) FROM [Nps] WHERE Score < 6) / (SELECT COUNT(*) FROM [Nps]),
                            0
                    ) AS PorcentagemDiferenca;";

            return await connection.QueryFirstOrDefaultAsync<int>(sql);
        }
    }
}
