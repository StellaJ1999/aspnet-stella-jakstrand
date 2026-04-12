namespace Presentation.WebApp.Models;

public sealed record TrainingSessionViewModel(
    Guid Id,
    string Name,
    DateTime StartTime,
    DateTime EndTime,
    int MaxParticipants
);
