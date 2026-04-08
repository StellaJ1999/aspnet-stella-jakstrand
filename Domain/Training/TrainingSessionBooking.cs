
namespace Domain.Training;

public class TrainingSessionBooking
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TrainingSessionId { get; set; }
    public string UserId { get; set; } = null!;

    public DateTime BookedAtUtc { get; set; } = DateTime.UtcNow;

    public TrainingSession TrainingSession { get; set; } = null!;
}
