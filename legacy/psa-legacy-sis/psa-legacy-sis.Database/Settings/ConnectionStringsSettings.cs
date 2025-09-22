namespace psa_legacy_sis.Database.Settings;

public sealed record class ConnectionStringsSettings
{
    public const string SectionName = "ConnectionStrings";

    public required string Legado { get; init; }
}
