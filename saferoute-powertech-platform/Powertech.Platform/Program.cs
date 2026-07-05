using System.Text;
using Cortex.Mediator.Commands;
using Cortex.Mediator.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Powertech.Platform.Fleet.Application.Internal.CommandServices;
using Powertech.Platform.Fleet.Application.Internal.QueryServices;
using Powertech.Platform.Fleet.Application.QueryServices;
using Powertech.Platform.Fleet.Domain.Repositories;
using Powertech.Platform.Fleet.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Powertech.Platform.Fleet.Infrastructure.Persistence.EntityFrameworkCore.Seeding;
using Powertech.Platform.Iam.Application.CommandServices;
using Powertech.Platform.Iam.Application.Internal.CommandServices;
using Powertech.Platform.Iam.Application.Internal.OutboundServices;
using Powertech.Platform.Iam.Application.Internal.QueryServices;
using Powertech.Platform.Iam.Application.QueryServices;
using Powertech.Platform.Iam.Domain.Repositories;
using Powertech.Platform.Iam.Infrastructure.Hashing.BCrypt.Services;
using Powertech.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Powertech.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Seeding;
using Powertech.Platform.Iam.Infrastructure.Tokens.Jwt.Configuration;
using Powertech.Platform.Iam.Infrastructure.Tokens.Jwt.Services;
using Powertech.Platform.Notifications.Application.CommandServices;
using Powertech.Platform.Notifications.Application.Internal.CommandServices;
using Powertech.Platform.Notifications.Application.Internal.QueryServices;
using Powertech.Platform.Notifications.Application.QueryServices;
using Powertech.Platform.Notifications.Domain.Repositories;
using Powertech.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Powertech.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Seeding;
using Powertech.Platform.Resources.Errors;
using Powertech.Platform.Resources.Shared;
using Powertech.Platform.Shared.Domain.Repositories;
using Powertech.Platform.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Mediator.Cortex.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Powertech.Platform.Shared.Infrastructure.Pipeline.Middleware.Extensions;
using Powertech.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Powertech.Platform.Stakeholder.Application.CommandServices;
using Powertech.Platform.Stakeholder.Application.Internal.CommandServices;
using Powertech.Platform.Stakeholder.Application.Internal.QueryServices;
using Powertech.Platform.Stakeholder.Application.QueryServices;
using Powertech.Platform.Stakeholder.Domain.Repositories;
using Powertech.Platform.Stakeholder.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Powertech.Platform.Stakeholder.Infrastructure.Persistence.EntityFrameworkCore.Seeding;
using Powertech.Platform.Subscription.Application.CommandServices;
using Powertech.Platform.Subscription.Application.Internal.CommandServices;
using Powertech.Platform.Subscription.Application.Internal.QueryServices;
using Powertech.Platform.Subscription.Application.QueryServices;
using Powertech.Platform.Subscription.Domain.Repositories;
using Powertech.Platform.Subscription.Infraestructure.Persistence.EntityFrameworkCore.Seeding;
using Powertech.Platform.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Powertech.Platform.Trip.Application.CommandServices;
using Powertech.Platform.Trip.Application.Internal;
using Powertech.Platform.Trip.Application.Internal.QueryServices;
using Powertech.Platform.Trip.Application.QueryServices;
using Powertech.Platform.Trip.Domain.Repositories;
using Powertech.Platform.Trip.Infrastructure.Persistence.EntityFrameworkCore.Seeding;
using Powertech.Platform.Trip.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Routing & Controllers
// ---------------------------------------------------------------------------
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization();

builder.Services.AddProblemDetails();

// ---------------------------------------------------------------------------
// CORS
// ---------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ---------------------------------------------------------------------------
// Database (MySQL via EF Core)
// ---------------------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// ---------------------------------------------------------------------------
// Authentication (JWT Bearer) — issued by the Iam bounded context
// ---------------------------------------------------------------------------
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));

var tokenSecret = builder.Configuration["TokenSettings:Secret"];
if (string.IsNullOrWhiteSpace(tokenSecret))
    throw new InvalidOperationException("TokenSettings:Secret is not set in the configuration.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// ---------------------------------------------------------------------------
// Localization
// ---------------------------------------------------------------------------
builder.Services.AddLocalization();
builder.Services.AddSingleton<IStringLocalizer<ErrorMessages>, StringLocalizer<ErrorMessages>>();
builder.Services.AddSingleton<IStringLocalizer<CommonMessages>, StringLocalizer<CommonMessages>>();

// Custom RFC 7807 problem details factory.
builder.Services.AddSingleton<ProblemDetailsFactory>();

// ---------------------------------------------------------------------------
// Swagger / OpenAPI
// ---------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Powertech-Platform",
            Version = "v1",
            Description = "Powertech Platform API",
            License = new OpenApiLicense
            {
                Name = "Apache 2.0",
                Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
            }
        });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] });
    options.EnableAnnotations();
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});

// ---------------------------------------------------------------------------
// Dependency Injection per Bounded Context
// ---------------------------------------------------------------------------

// Shared
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Stakeholder bounded context
builder.Services.AddScoped<IParentRepository, ParentRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IStudentGroupRepository, StudentGroupRepository>();
builder.Services.AddScoped<IStakeholderCommandService, StakeholderCommandService>();
builder.Services.AddScoped<IStakeholderQueryService, StakeholderQueryService>();

// Trip bounded context
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<ITripCommandService, TripCommandService>();
builder.Services.AddScoped<ITripQueryService, TripQueryService>();

// Fleet bounded context
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<IRouteCommandService, RouteCommandService>();
builder.Services.AddScoped<IRouteQueryService, RouteQueryService>();

// Subscription bounded context
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();

// Notifications bounded context
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationCommandService, NotificationCommandService>();
builder.Services.AddScoped<INotificationQueryService, NotificationQueryService>();

// Iam bounded context
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IIamCommandService, IamCommandService>();
builder.Services.AddScoped<IIamQueryService, IamQueryService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// ---------------------------------------------------------------------------
// Mediator (Cortex) — command pipeline behaviors and event handling
// ---------------------------------------------------------------------------
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(LoggingCommandBehavior<>));
builder.Services.AddCortexMediator([typeof(Program)]);

var app = builder.Build();

// Apply pending migrations on startup (safe to call even when the schema is up to date).
// Wrapped so a database connectivity/credential problem does not prevent the API (and Swagger)
// from starting: the app boots, surfaces a clear log entry, and database-backed endpoints fail
// individually until the connection is fixed.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var startupLogger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        startupLogger.LogInformation("Database migrations applied successfully.");

        // Seed demo data on an empty database, in dependency order across bounded contexts:
        // identity/org first, then stakeholders, then plans/subscription, routes, trips and
        // finally notifications that reference the seeded parent and trip.
        var hashingService = services.GetRequiredService<IHashingService>();
        await IamSeeder.SeedAsync(context, hashingService);
        await StakeholderSeeder.SeedAsync(context);
        await SubscriptionSeeder.SeedAsync(context);
        await FleetSeeder.SeedAsync(context);
        await TripSeeder.SeedAsync(context);
        await NotificationSeeder.SeedAsync(context);
        startupLogger.LogInformation("Seed data ensured.");
    }
    catch (Exception ex)
    {
        startupLogger.LogError(ex,
            "Database migration failed at startup. The API will still start, but database-backed " +
            "endpoints will fail until the connection string in appsettings is corrected.");
    }
}

// ---------------------------------------------------------------------------
// HTTP request pipeline
// ---------------------------------------------------------------------------
app.UseGlobalExceptionHandler();

var supportedCultures = new[] { "en", "es" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllPolicy");
// HTTPS redirection only outside Development so local http://localhost:8080 calls
// (Swagger and the Vue frontend) are not redirected to the self-signed https port.
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
