namespace Safer_Route_Platform.Stakeholder.Domain.Model.Commands;

public record CreateStudentGroupCommand(Guid OrganizationId, string Name);
