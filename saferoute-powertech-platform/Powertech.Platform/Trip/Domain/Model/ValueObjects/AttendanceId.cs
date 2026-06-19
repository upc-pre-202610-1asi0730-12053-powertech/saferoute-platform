namespace Powertech.Platform.Trip.Domain.Model.ValueObjects;

/// <summary>
///     Value object that identifies an <c>Attendance</c> entity inside the Trip aggregate.
/// </summary>
/// <remarks>
///     Local identity of a child entity. It is owned by the Trip context and never referenced
///     from the outside, therefore it is not part of the Shared context.
/// </remarks>
/// <param name="Identifier">The unique identifier of the attendance record.</param>
public record AttendanceId(Guid Identifier)
{
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    public AttendanceId() : this(Guid.NewGuid())
    {
    }

    /// <summary>Factory method that creates a brand new <see cref="AttendanceId" />.</summary>
    public static AttendanceId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Identifier.ToString();
}