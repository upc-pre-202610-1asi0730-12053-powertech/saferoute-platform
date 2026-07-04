using Powertech.Platform.Iam.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;

namespace Powertech.Platform.Iam.Domain.Model.Aggregates;

/// <summary>
///     User Aggregate Root.
/// </summary>
/// <remarks>
///     Represents an account that can sign in to the platform: an organization admin, a
///     driver or a parent. Drivers and parents are further modeled as stakeholder profiles
///     in the <c>Stakeholder</c> bounded context, linked back to this aggregate through the
///     shared <see cref="UserId" /> value object.
/// </remarks>
public class User
{
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    protected User()
    {
        Id = new UserId();
        OrganizationId = null;
        FullName = new FullName();
        Email = new Email();
        PasswordHash = new PasswordHash();
        Role = new RoleTier();
    }

    /// <summary>Creates a new user account with an already-hashed password.</summary>
    public User(OrganizationId? organizationId, FullName fullName, Email email, PasswordHash passwordHash,
        RoleTier role)
    {
        Id = UserId.New();
        OrganizationId = organizationId;
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public UserId Id { get; private set; }

    public OrganizationId? OrganizationId { get; private set; }

    public FullName FullName { get; private set; }

    public Email Email { get; private set; }

    public PasswordHash PasswordHash { get; private set; }

    public RoleTier Role { get; private set; }

    public bool IsAdmin() => Role.IsAdmin();

    public string GetFullName() => FullName.ToString();

    public string GetEmail() => Email.ToString();
}