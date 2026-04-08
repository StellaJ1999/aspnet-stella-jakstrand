
using Application.Training.Inputs;
using Domain.Training;

namespace Application.Abstractions.Training;

public interface ITrainingSessionRepository
{
    Task<bool> CreateTrainingSessionAsync(TrainingSessionInput model);
    Task<bool> UpdateTrainingSessionAsync(TrainingSessionInput model);
    Task<bool> DeleteTrainingSessionAsync(Guid sessionId);
    Task<IReadOnlyList<TrainingSession>> GetAllAsync();
    Task<TrainingSession?> GetByIdAsync(Guid id);
}
 