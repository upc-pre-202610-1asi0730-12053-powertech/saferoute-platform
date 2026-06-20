using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Stakeholder.Domain.Model.Commands;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;
using Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

namespace Powertech.Platform.Stakeholder.Domain.Model.Aggregates;

public class Parent
{
    private readonly List<Child> _children = [];

    protected Parent()
    {
        Id = new ParentId();
        OrganizationId = new OrganizationId();
        UserId = new UserId();
        FullName = new FullName(string.Empty, string.Empty, false);
        Email = new Email(string.Empty, false);
        PhoneNumber = new PhoneNumber(string.Empty, false);
    }

    public Parent(OrganizationId organizationId, UserId userId, FullName fullName, Email email, PhoneNumber phoneNumber)
    {
        Id = ParentId.New();
        OrganizationId = organizationId;
        UserId = userId;
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public Parent(CreateParentCommand command)
        : this(new OrganizationId(command.OrganizationId), new UserId(command.UserId),
            new FullName(command.FirstName, command.LastName), new Email(command.Email),
            new PhoneNumber(command.PhoneNumber))
    {
    }

    public ParentId Id { get; private set; }

    public OrganizationId OrganizationId { get; private set; }

    public UserId UserId { get; private set; }

    public FullName FullName { get; private set; }

    public Email Email { get; private set; }

    public PhoneNumber PhoneNumber { get; private set; }

    public IReadOnlyCollection<Child> Children => _children;

    public void AddChild(Child child) => _children.Add(child);

    public void RemoveChild(ChildId childId)
    {
        var child = _children.FirstOrDefault(c => c.Id == childId);
        if (child is not null) _children.Remove(child);
    }

    public IReadOnlyCollection<Child> GetChildren() => Children;

    public string GetFullName() => FullName.ToString();

    public string GetEmail() => Email.ToString();

    public void Update(FullName fullName, Email email, PhoneNumber phoneNumber)
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}
