using System.Data.Common;

namespace NPS.Infrastructure.Data;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateConnectionAsync();
}