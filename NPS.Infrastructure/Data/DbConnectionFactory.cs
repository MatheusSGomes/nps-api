using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace NPS.Infrastructure.Data;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentException(nameof(connectionString), "String de conexão não pode ser nula");
    }

    public DbConnection CreateConnection()
    {
        if (string.IsNullOrEmpty(_connectionString))
            throw new Exception("String de conexão não pode ser nula");

        var sqlConnection = new SqlConnection(_connectionString);
        return sqlConnection;
    }

    public async Task<DbConnection> CreateConnectionAsync()
    {
        if (string.IsNullOrEmpty(_connectionString))
            throw new Exception("String de conexão não pode ser nula");

        var sqlConnection = new SqlConnection(_connectionString);

        try
        {
            await sqlConnection.OpenAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Erro ao abrir conexão com o banco de dados.", e);
        }

        return sqlConnection;
    }
}
