
using Domain.Common;

namespace Domain.Training;

public class TrainingSessionBooking
{
    private TrainingSessionBooking() { }

    internal TrainingSessionBooking(Guid sessionId, string userId, DateTime bookedAtUtc)
    {
        if (sessionId == default)
            throw new DomainException("TrainingSessionId is required.");

        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("UserId is required.");

        Id = Guid.NewGuid();
        TrainingSessionId = sessionId;
        UserId = userId.Trim();
        BookedAtUtc = bookedAtUtc;
    }

    public static TrainingSessionBooking Create(Guid sessionId, string userId, DateTime bookedAtUtc)
        => new(sessionId, userId, bookedAtUtc);
    public Guid Id { get; private set; }
    public Guid TrainingSessionId { get; private set; }
    public string UserId { get; private set; } = null!;
    public TrainingSession TrainingSession { get; private set; } = null!;
    public DateTime BookedAtUtc { get; private set; }
}
