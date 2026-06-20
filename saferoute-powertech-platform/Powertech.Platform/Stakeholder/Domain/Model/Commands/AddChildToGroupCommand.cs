namespace Powertech.Platform.Stakeholder.Domain.Model.Commands;

public record AddChildToGroupCommand(Guid StudentGroupId, Guid ChildId);
