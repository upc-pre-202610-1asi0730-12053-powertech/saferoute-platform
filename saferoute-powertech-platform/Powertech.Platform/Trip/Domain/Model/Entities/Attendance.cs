using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Trip.Domain.Model.ValueObjects;

namespace Powertech.Platform.Trip.Domain.Model.Entities;

/// <summary>
///     Entity that records the boarding attendance of a single child during a trip.
/// </summary>
/// <remarks>
///     <para>
///         <c>Attendance</c> is a child entity of the <see cref="Aggregates.Trip" /> aggregate. It is
///         created and mutated exclusively through the aggregate root, never directly, to preserve
///         the aggregate's invariants.
///     </para>
/// </remarks>
public class Attendance
{
    /// <summary>Parameterless constructor required by EF Core materialization.</summary>
    protected Attendance()
    {
        Id = new AttendanceId();
        ChildId = new ChildId();
        BoardingState = new BoardingState();
    }

    /// <summary>
    ///     Creates a new attendance record for a child with an initial boarding state.
    /// </summary>
    /// <param name="childId">The child whose boarding is tracked.</param>
    /// <param name="boardingState">The initial boarding state.</param>
    public Attendance(ChildId childId, BoardingState boardingState)
    {
        Id = AttendanceId.New();
        ChildId = childId;
        BoardingState = boardingState;
        BoardedAt = boardingState.IsBoarded() ? DateTimeOffset.UtcNow : null;
    }

    /// <summary>Local identity of the attendance record within the aggregate.</summary>
    public AttendanceId Id { get; private set; }

    /// <summary>The child this attendance belongs to (reference to the Stakeholder context).</summary>
    public ChildId ChildId { get; private set; }

    /// <summary>The current boarding state of the child.</summary>
    public BoardingState BoardingState { get; private set; }

    /// <summary>The moment the child boarded, when applicable; otherwise <c>null</c>.</summary>
    public DateTimeOffset? BoardedAt { get; private set; }

    /// <summary>
    ///     Updates the boarding state and stamps the boarding time when the child boards.
    /// </summary>
    /// <param name="state">The new boarding state.</param>
    public void UpdateBoardingState(BoardingState state)
    {
        BoardingState = state;
        BoardedAt = state.IsBoarded() ? DateTimeOffset.UtcNow : BoardedAt;
    }

    /// <summary>Returns <c>true</c> when the child has boarded.</summary>
    public bool IsBoarded() => BoardingState.IsBoarded();

    /// <summary>Returns the boarding time, if any.</summary>
    public DateTimeOffset? GetBoardingTime() => BoardedAt;
}