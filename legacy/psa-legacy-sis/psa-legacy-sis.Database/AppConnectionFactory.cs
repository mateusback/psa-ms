using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using psa_legacy_sis.Database.Settings;

namespace psa_legacy_sis.Database;

internal sealed class AppConnectionFactory : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public AppConnectionFactory([FromKeyedServices(IoC.DataSourceKey)] NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<NpgsqlConnection> CreateOpenAsync(CancellationToken cancellationToken = default)
    {
        return await _dataSource.OpenConnectionAsync(cancellationToken);
    }
}
