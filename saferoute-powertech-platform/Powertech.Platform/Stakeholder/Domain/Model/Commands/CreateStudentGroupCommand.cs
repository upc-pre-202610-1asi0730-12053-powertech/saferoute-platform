namespace Powertech.Platform.Stakeholder.Domain.Model.Commands;

public record CreateStudentGroupCommand(Guid OrganizationId, string Name);
