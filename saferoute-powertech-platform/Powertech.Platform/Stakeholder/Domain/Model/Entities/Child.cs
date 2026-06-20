using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

namespace Powertech.Platform.Stakeholder.Domain.Model.Entities;

public class Child
{
    protected Child()
    {
        Id = new ChildId();
        FullName = new FullName(string.Empty, string.Empty, false);
        EnrollmentState = new ChildEnrollmentState();
    }

    public Child(FullName fullName, int age)
    {
        if (age < 0) throw new ArgumentException("Age cannot be negative.", nameof(age));
        Id = ChildId.New();
        FullName = fullName;
        Age = age;
        EnrollmentState = new ChildEnrollmentState(ChildEnrollmentState.Active);
    }

    public ChildId Id { get; private set; }

    public FullName FullName { get; private set; }

    public int Age { get; private set; }

    public ChildEnrollmentState EnrollmentState { get; private set; }

    public void Enroll() => EnrollmentState = new ChildEnrollmentState(ChildEnrollmentState.Active);

    public void Unenroll() => EnrollmentState = new ChildEnrollmentState(ChildEnrollmentState.Inactive);

    public bool IsEnrolled() => EnrollmentState.IsActive();

    public string GetFullName() => FullName.ToString();
}
