namespace Presentation.WebApp.Models;

public sealed record TrainingSessionListItemViewModel(
    TrainingSessionViewModel Session,
    bool IsBooked,
    int BookedCount,
    bool IsFull
);
