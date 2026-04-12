namespace Presentation.WebApp.Areas.Admin.Models;

public sealed record TrainingSessionScheduleItemViewModel(
    Guid Id,
    string Name,
    DateTime StartTime,
    DateTime EndTime,
    int MaxParticipants
);
