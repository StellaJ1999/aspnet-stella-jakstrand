namespace Application.Training.Inputs;

public sealed record TrainingSessionInput 
(
    string Title,
    string Description,
    DateTime StartTime,
    DateTime EndTime,
    int Capacity
)
{
    public Guid Id { get; private set; }
    public DateTime CreatedUtc { get; private set; }

    public void SetId(Guid id) => Id = id;
    public void SetCreatedUtc(DateTime createdUtc) => CreatedUtc = createdUtc;
}
