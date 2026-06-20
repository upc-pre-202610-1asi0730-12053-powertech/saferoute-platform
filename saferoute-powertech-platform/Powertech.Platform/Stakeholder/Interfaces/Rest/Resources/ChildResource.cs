namespace Powertech.Platform.Stakeholder.Interfaces.Rest.Resources;

public record ChildResource(Guid Id, string FirstName, string LastName, string FullName, int Age, string EnrollmentState);
