using Application.Abstractions.Training;
using Application.Training.Inputs;
using Domain.Training;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal class TrainingSessionRepository(PersistenceContext db)
    : RepositoryBase(db), ITrainingSessionRepository
{
    public async Task<bool> CreateTrainingSessionAsync(TrainingSessionInput model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = TrainingSession.Create(
            id: model.Id,
            name: model.Name,
            startTime: model.StartTime,
            endTime: model.EndTime,
            maxParticipants: model.MaxParticipants,
            createdUtc: model.CreatedUtc
        );

        Db.TrainingSessions.Add(entity);
        return await SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteTrainingSessionAsync(Guid sessionId)
    {
        var entity = await Db.TrainingSessions.FirstOrDefaultAsync(x => x.Id == sessionId);
        if (entity is null)
            return false;

        Db.TrainingSessions.Remove(entity);
        return await SaveChangesAsync() > 0;
    }

    public async Task<IReadOnlyList<TrainingSession>> GetAllAsync()
    {
        return await Db.TrainingSessions
            .Include(x => x.Bookings)
            .AsNoTracking()
            .OrderBy(x => x.StartTime)
            .ToListAsync();
    }

    public async Task<TrainingSession?> GetByIdAsync(Guid id)
    {
        return await Db.TrainingSessions
            .Include(x => x.Bookings)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UpdateTrainingSessionAsync(TrainingSessionInput model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = await Db.TrainingSessions.FirstOrDefaultAsync(x => x.Id == model.Id);
        if (entity is null)
            return false;

        entity.Update(
            name: model.Name,
            startTime: model.StartTime,
            endTime: model.EndTime,
            maxParticipants: model.MaxParticipants
        );

        return await SaveChangesAsync() > 0;
    }
}
