using Application.Training.Inputs;
using Domain.Training;

namespace Application.Abstractions.Training;

public interface ITrainingSessionService
{
    Task<bool> CreateTrainingSessionAsync(TrainingSessionInput input);
    Task<IReadOnlyList<TrainingSession>> GetAllTrainingSessionsAsync();
    Task<TrainingSession?> GetTrainingSessionByIdAsync(Guid id);
    Task<bool> UpdateTrainingSessionAsync(Guid id, TrainingSessionInput input);
    Task<bool> DeleteTrainingSessionAsync(Guid id);
}
