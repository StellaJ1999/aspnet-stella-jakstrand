using Application.Abstractions.Training;
using Application.Training.Inputs;
using Domain.Training;

namespace Application.Training.Services;

public sealed class TrainingSessionService(ITrainingSessionRepository repo) : ITrainingSessionService
{
    public async Task<bool> CreateTrainingSessionAsync(TrainingSessionInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        input.SetId(Guid.NewGuid());
        input.SetCreatedUtc(DateTime.UtcNow);

        // Let `DomainException` bubble up so the presentation layer can show a useful message.
        // Don't swallow unexpected exceptions either.
        return await repo.CreateTrainingSessionAsync(input);
    }

    public Task<IReadOnlyList<TrainingSession>> GetAllTrainingSessionsAsync()
        => repo.GetAllAsync();

    public Task<TrainingSession?> GetTrainingSessionByIdAsync(Guid id)
        => repo.GetByIdAsync(id);

    public async Task<bool> UpdateTrainingSessionAsync(Guid id, TrainingSessionInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        input.SetId(id);

        return await repo.UpdateTrainingSessionAsync(input);
    }

    public Task<bool> DeleteTrainingSessionAsync(Guid id)
        => repo.DeleteTrainingSessionAsync(id);
}
