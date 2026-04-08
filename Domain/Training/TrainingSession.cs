namespace Domain.Training;

public class TrainingSession
{
    public Guid id { get; set; }
    public string Name { get; set; } = null!;

    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public int MaxParticipants { get; set; } = 20;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public List<TrainingSessionBooking> Bookings { get; set; } = new List<TrainingSessionBooking>();
}
