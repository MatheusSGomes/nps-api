using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace NPS.Infrastructure.Data.Queries;

public class Query
{
    private readonly IConfiguration _configuration;

    public Query(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected async Task<T> QuerySingleAsync<T>(string sql, object parameters = null)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        await using (var connection = new SqlConnection(connectionString))
        {
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param: parameters);
        }
    }
}
