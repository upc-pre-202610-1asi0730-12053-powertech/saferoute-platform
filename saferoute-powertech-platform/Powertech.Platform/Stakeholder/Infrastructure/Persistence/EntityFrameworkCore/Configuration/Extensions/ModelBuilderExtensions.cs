using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Stakeholder.Domain.Model.Aggregates;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;
using Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

namespace Powertech.Platform.Stakeholder.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyStakeholderConfiguration(this ModelBuilder builder)
    {
        var childIdsConverter = new ValueConverter<List<Guid>, string>(
            ids => string.Join(',', ids),
            value => value.Length == 0
                ? new List<Guid>()
                : value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList());

        var childIdsComparer = new ValueComparer<List<Guid>>(
            (left, right) => (left ?? new List<Guid>()).SequenceEqual(right ?? new List<Guid>()),
            ids => ids.Aggregate(0, (hash, id) => HashCode.Combine(hash, id.GetHashCode())),
            ids => ids.ToList());

        builder.Entity<Parent>(parent =>
        {
            parent.ToTable("Parents");
            parent.HasKey(p => p.Id);
            parent.Property(p => p.Id).HasConversion(id => id.Identifier, value => new ParentId(value))
                .ValueGeneratedNever();
            parent.Property(p => p.OrganizationId).HasConversion(id => id.Identifier, value => new OrganizationId(value));
            parent.Property(p => p.UserId).HasConversion(id => id.Identifier, value => new UserId(value));
            parent.OwnsOne(p => p.FullName, fullName =>
            {
                fullName.WithOwner().HasForeignKey("Id");
                fullName.Property(f => f.FirstName).HasMaxLength(FullName.MaxLength).IsRequired();
                fullName.Property(f => f.LastName).HasMaxLength(FullName.MaxLength).IsRequired();
            });
            parent.Property(p => p.Email).HasConversion(email => email.Value, value => new Email(value))
                .HasMaxLength(Email.MaxLength);
            parent.Property(p => p.PhoneNumber).HasConversion(phone => phone.Value, value => new PhoneNumber(value))
                .HasMaxLength(PhoneNumber.MaxLength);
            parent.OwnsMany(p => p.Children, child =>
            {
                child.WithOwner().HasForeignKey("ParentId");
                child.Property(c => c.Id).HasConversion(id => id.Identifier, value => new ChildId(value))
                    .ValueGeneratedNever();
                child.HasKey(c => c.Id);
                child.OwnsOne(c => c.FullName, fullName =>
                {
                    fullName.WithOwner().HasForeignKey("Id");
                    fullName.Property(f => f.FirstName).HasMaxLength(FullName.MaxLength).IsRequired();
                    fullName.Property(f => f.LastName).HasMaxLength(FullName.MaxLength).IsRequired();
                });
                child.Property(c => c.Age).IsRequired();
                child.Property(c => c.EnrollmentState)
                    .HasConversion(state => state.Value, value => new ChildEnrollmentState(value))
                    .HasMaxLength(20);
            });
        });

        builder.Entity<Driver>(driver =>
        {
            driver.ToTable("Drivers");
            driver.HasKey(d => d.Id);
            driver.Property(d => d.Id).HasConversion(id => id.Identifier, value => new DriverId(value))
                .ValueGeneratedNever();
            driver.Property(d => d.OrganizationId).HasConversion(id => id.Identifier, value => new OrganizationId(value));
            driver.Property(d => d.UserId).HasConversion(id => id.Identifier, value => new UserId(value));
            driver.OwnsOne(d => d.FullName, fullName =>
            {
                fullName.WithOwner().HasForeignKey("Id");
                fullName.Property(f => f.FirstName).HasMaxLength(FullName.MaxLength).IsRequired();
                fullName.Property(f => f.LastName).HasMaxLength(FullName.MaxLength).IsRequired();
            });
            driver.Property(d => d.Email).HasConversion(email => email.Value, value => new Email(value))
                .HasMaxLength(Email.MaxLength);
            driver.Property(d => d.PhoneNumber).HasConversion(phone => phone.Value, value => new PhoneNumber(value))
                .HasMaxLength(PhoneNumber.MaxLength);
            driver.Property(d => d.LicenseNumber)
                .HasConversion(license => license.Value, value => new LicenseNumber(value))
                .HasMaxLength(LicenseNumber.MaxLength);
            driver.Property(d => d.Available).IsRequired();
        });

        builder.Entity<StudentGroup>(group =>
        {
            group.ToTable("StudentGroups");
            group.HasKey(g => g.Id);
            group.Property(g => g.Id).HasConversion(id => id.Identifier, value => new StudentGroupId(value))
                .ValueGeneratedNever();
            group.Property(g => g.OrganizationId).HasConversion(id => id.Identifier, value => new OrganizationId(value));
            group.Property(g => g.Name).HasMaxLength(100).IsRequired();
            group.Property(g => g.IsFinalizedValue).IsRequired();
            var childIdsProperty = group.Property(g => g.ChildIds)
                .HasConversion(childIdsConverter)
                .HasColumnName("child_ids")
                .Metadata;
            childIdsProperty.SetValueComparer(childIdsComparer);
            group.Ignore(g => g.Children);
        });
    }
}
