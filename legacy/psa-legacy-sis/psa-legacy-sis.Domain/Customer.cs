using psa_legacy_sis.Domain.Base;

namespace psa_legacy_sis.Domain;

public class Customer : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}