namespace Safer_Route_Platform.Stakeholder.Domain.Model.Commands;

public record RemoveChildFromGroupCommand(Guid StudentGroupId, Guid ChildId);
