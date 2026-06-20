namespace Powertech.Platform.Stakeholder.Domain.Model.Commands;

public record RemoveChildFromGroupCommand(Guid StudentGroupId, Guid ChildId);
