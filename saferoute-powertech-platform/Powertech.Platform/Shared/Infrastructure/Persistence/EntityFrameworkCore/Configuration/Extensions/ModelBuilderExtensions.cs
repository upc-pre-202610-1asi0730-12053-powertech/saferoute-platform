using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Entities;

namespace Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     Model builder extensions for the database context
/// </summary>
public static class ModelBuilderExtensions
{
    public static void ApplySharedConfiguration(this ModelBuilder builder)
    {
        builder.Entity<VehicleCatalogItem>(vehicle =>
        {
            vehicle.ToTable("Vehicle");
            vehicle.HasKey(v => v.Id);
            vehicle.Property(v => v.Id).ValueGeneratedNever();
            vehicle.Property(v => v.OrganizationId).IsRequired();
            vehicle.Property(v => v.Plate).IsRequired().HasMaxLength(20);
            vehicle.Property(v => v.Model).IsRequired().HasMaxLength(80);
            vehicle.Property(v => v.Capacity).IsRequired();
            vehicle.Property(v => v.Status).IsRequired().HasMaxLength(20);
        });
    }

    /// <summary>
    ///     Use snake case naming convention for the database context
    /// </summary>
    /// <param name="builder">
    ///     The model builder for the database context
    /// </param>
    public static void UseSnakeCaseNamingConvention(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName)) entity.SetTableName(tableName.ToPlural().ToSnakeCase());

            foreach (var property in entity.GetProperties())
                property.SetColumnName(property.GetColumnName().ToSnakeCase());

            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrEmpty(keyName)) key.SetName(keyName.ToSnakeCase());
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                var foreignKeyName = foreignKey.GetConstraintName();
                if (!string.IsNullOrEmpty(foreignKeyName)) foreignKey.SetConstraintName(foreignKeyName.ToSnakeCase());
            }

            foreach (var index in entity.GetIndexes())
            {
                var indexDatabaseName = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(indexDatabaseName)) index.SetDatabaseName(indexDatabaseName.ToSnakeCase());
            }
        }
    }
}
