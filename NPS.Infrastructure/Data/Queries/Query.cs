using Dapper;
using Microsoft.Extensions.Configuration;

namespace NPS.Infrastructure.Data.Queries;

public class Query
{
    private readonly string? _connectionString;

    public Query(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    protected async Task<T> QueryFirstAsync<T>(string sql, object parameters = null)
    {
        var dbConnectionFactory = new DbConnectionFactory(_connectionString);
        using (var connection = await dbConnectionFactory.CreateConnectionAsync())
        return await connection.QueryFirstOrDefaultAsync<T>(sql, param: parameters);
    }

    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
    {
        var dbConnectionFactory = new DbConnectionFactory(_connectionString);
        using (var connection = await dbConnectionFactory.CreateConnectionAsync())
        return await connection.QueryAsync<T>(sql, param: parameters);
    }
}
