using Powertech.Platform.Iam.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;

namespace Powertech.Platform.Iam.Domain.Model.Aggregates;

/// <summary>
///     Organization Aggregate Root.
/// </summary>
/// <remarks>
///     Represents the tenant that owns drivers, parents, routes and trips across the other
///     bounded contexts. Other contexts reference it only through the shared
///     <see cref="OrganizationId" /> value object.
/// </remarks>
public partial class Organization
{
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    protected Organization()
    {
        Id = new OrganizationId();
        Name = new OrganizationName();
        Status = OrganizationStatus.CreateActive();
    }

    /// <summary>Creates a new, active organization.</summary>
    public Organization(OrganizationName name)
    {
        Id = OrganizationId.New();
        Name = name;
        Status = OrganizationStatus.CreateActive();
    }

    public OrganizationId Id { get; private set; }

    public OrganizationName Name { get; private set; }

    public OrganizationStatus Status { get; private set; }

    public void UpdateName(OrganizationName name) => Name = name;

    public void Suspend() => Status = OrganizationStatus.CreateSuspended();

    public void Activate() => Status = OrganizationStatus.CreateActive();

    public bool IsActive() => Status.IsActive();

    public string GetName() => Name.ToString();
}
