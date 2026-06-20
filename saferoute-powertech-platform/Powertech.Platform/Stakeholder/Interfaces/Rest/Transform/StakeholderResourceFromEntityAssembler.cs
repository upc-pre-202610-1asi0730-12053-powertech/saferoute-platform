using Powertech.Platform.Stakeholder.Domain.Model.Aggregates;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;
using Powertech.Platform.Stakeholder.Interfaces.Rest.Resources;

namespace Powertech.Platform.Stakeholder.Interfaces.Rest.Transform;

public static class StakeholderResourceFromEntityAssembler
{
    public static ParentResource ToResourceFromEntity(Parent parent) =>
        new(parent.Id.Identifier, parent.OrganizationId.Identifier, parent.UserId.Identifier,
            parent.FullName.FirstName, parent.FullName.LastName, parent.FullName.ToString(), parent.Email.Value,
            parent.PhoneNumber.Value, parent.Children.Select(ToResourceFromEntity));

    public static ChildResource ToResourceFromEntity(Child child) =>
        new(child.Id.Identifier, child.FullName.FirstName, child.FullName.LastName, child.FullName.ToString(),
            child.Age, child.EnrollmentState.Value);

    public static DriverResource ToResourceFromEntity(Driver driver) =>
        new(driver.Id.Identifier, driver.OrganizationId.Identifier, driver.UserId.Identifier,
            driver.FullName.FirstName, driver.FullName.LastName, driver.FullName.ToString(), driver.Email.Value,
            driver.PhoneNumber.Value, driver.LicenseNumber.Value, driver.Available);

    public static StudentGroupResource ToResourceFromEntity(StudentGroup group) =>
        new(group.Id.Identifier, group.OrganizationId.Identifier, group.Name, group.ChildIds, group.GetChildCount(),
            group.IsFinalized());
}
