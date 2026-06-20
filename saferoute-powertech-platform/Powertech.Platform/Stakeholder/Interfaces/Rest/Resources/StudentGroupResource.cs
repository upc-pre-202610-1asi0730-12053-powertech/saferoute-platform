namespace Powertech.Platform.Stakeholder.Interfaces.Rest.Resources;

public record StudentGroupResource(
    Guid Id,
    Guid OrganizationId,
    string Name,
    IEnumerable<Guid> ChildIds,
    int ChildCount,
    bool IsFinalized);
