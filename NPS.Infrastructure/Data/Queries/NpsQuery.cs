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
            var sql = "SELECT AVG(Score) AS Media FROM [Nps];";
            return await connection.QueryFirstOrDefaultAsync<int>(sql);
        }
    }
}
