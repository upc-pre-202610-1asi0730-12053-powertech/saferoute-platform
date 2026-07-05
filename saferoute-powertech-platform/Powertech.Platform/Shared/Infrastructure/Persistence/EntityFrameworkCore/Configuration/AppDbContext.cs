using Powertech.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using Powertech.Platform.Trip.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Powertech.Platform.Fleet.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Powertech.Platform.Stakeholder.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Powertech.Platform.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;


namespace Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;


/// <summary>
///     Application database context for the Learning Center Platform
/// </summary>
/// <param name="options">
///     The options for the database context
/// </param>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Apply audit timestamp interceptor for all IAuditableEntity implementations
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }


    /// <summary>
    ///     On creating the database model
    /// </summary>
    /// <remarks>
    ///     This method is used to create the database model for the application.
    /// </remarks>
    /// <param name="builder">
    ///     The model builder for the database context
    /// </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Per-bounded-context model configuration (declared in each context's Infrastructure layer).
        builder.ApplyIamConfiguration();
        builder.ApplyStakeholderConfiguration();
        builder.ApplyTripConfiguration();
        builder.ApplyFleetConfiguration();
        builder.ApplySubscriptionConfiguration();
        builder.ApplyNotificationConfiguration();

        // General Naming Convention for the database objects
        builder.UseSnakeCaseNamingConvention();
    }
}
