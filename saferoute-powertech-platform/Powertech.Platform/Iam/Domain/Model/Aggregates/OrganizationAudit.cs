using Powertech.Platform.Shared.Domain.Model.Entities;

namespace Powertech.Platform.Iam.Domain.Model.Aggregates;

/// <summary>
///     Audit trait for <see cref="Organization" />, populated automatically by the
///     <c>AuditableEntityInterceptor</c> on save.
/// </summary>
public partial class Organization : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
