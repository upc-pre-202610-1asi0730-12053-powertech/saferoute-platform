using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

namespace Powertech.Platform.Stakeholder.Domain.Model.Entities;

public class StudentGroup
{
    protected StudentGroup()
    {
        Id = new StudentGroupId();
        OrganizationId = new OrganizationId();
        Name = string.Empty;
        ChildIds = [];
    }

    public StudentGroup(OrganizationId organizationId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Student group name cannot be empty.", nameof(name));
        Id = StudentGroupId.New();
        OrganizationId = organizationId;
        Name = name.Trim();
        ChildIds = [];
    }

    public StudentGroupId Id { get; private set; }

    public OrganizationId OrganizationId { get; private set; }

    public string Name { get; private set; }

    public IReadOnlyCollection<ChildId> Children => ChildIds.Select(childId => new ChildId(childId)).ToList();

    public List<Guid> ChildIds { get; private set; }

    public bool IsFinalizedValue { get; private set; }

    public void AddChild(ChildId childId)
    {
        if (IsFinalizedValue) throw new InvalidOperationException("A finalized group cannot be modified.");
        if (ChildIds.All(existing => existing != childId.Identifier)) ChildIds.Add(childId.Identifier);
    }

    public void RemoveChild(ChildId childId)
    {
        if (IsFinalizedValue) throw new InvalidOperationException("A finalized group cannot be modified.");
        ChildIds.Remove(childId.Identifier);
    }

    public void Finalize() => IsFinalizedValue = true;

    public int GetChildCount() => ChildIds.Count;

    public bool IsFinalized() => IsFinalizedValue;

}
