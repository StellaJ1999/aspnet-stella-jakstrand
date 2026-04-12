using Domain.Common;

namespace Domain.Training;

public class TrainingSession
{
    private TrainingSession() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    public DateTime Date { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }

    public int MaxParticipants { get; private set; } = 20;
    public DateTime CreatedUtc { get; private set; } = DateTime.UtcNow;

    public List<TrainingSessionBooking> Bookings { get; private set; } = new();

    public static TrainingSession Create(
        Guid id,
        string name,
        DateTime startTime,
        DateTime endTime,
        int maxParticipants,
        DateTime createdUtc)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Training session name is required.");

        if (maxParticipants < 1)
            throw new DomainException("Max participants must be at least 1.");

        EnsureValidTimes(startTime, endTime);

        return new TrainingSession
        {
            Id = id == default ? Guid.NewGuid() : id,
            Name = name.Trim(),
            StartTime = startTime,
            EndTime = endTime,
            Date = startTime.Date,
            MaxParticipants = maxParticipants,
            CreatedUtc = createdUtc == default ? DateTime.UtcNow : createdUtc
        };
    }

    public void Update(string name, DateTime startTime, DateTime endTime, int maxParticipants)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Training session name is required.");

        if (maxParticipants < 1)
            throw new DomainException("Max participants must be at least 1.");

        EnsureValidTimes(startTime, endTime);

        Name = name.Trim();
        StartTime = startTime;
        EndTime = endTime;
        Date = startTime.Date;
        MaxParticipants = maxParticipants;
    }

    private static void EnsureValidTimes(DateTime startTime, DateTime endTime)
    {
        if (startTime >= endTime)
            throw new DomainException("Start time must be before end time.");
    }
}
