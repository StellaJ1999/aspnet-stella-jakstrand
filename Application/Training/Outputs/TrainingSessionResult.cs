namespace Application.Training.Outputs;

public sealed record TrainingSessionResult
(
    Guid Id,
    string Name,
    DateTime Date,
    DateTime StartTime,
    DateTime EndTime,
    int MaxParticipants,
    int CurrentParticipants,
    DateTime CreatedUtc
);
