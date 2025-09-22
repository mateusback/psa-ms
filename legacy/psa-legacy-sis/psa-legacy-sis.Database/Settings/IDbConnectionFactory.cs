using Npgsql;

namespace psa_legacy_sis.Database.Settings;

public interface IDbConnectionFactory
{
    Task<NpgsqlConnection> CreateOpenAsync(CancellationToken cancellationToken = default);
}
