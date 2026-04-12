using Application.Abstractions.Training;
using Domain.Training;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class TrainingSessionBookingRepository(PersistenceContext db)
    : RepositoryBase(db), ITrainingSessionBookingRepository
{
    public async Task<bool> AddAsync(Guid sessionId, string userId)
    {
        var booking = TrainingSessionBooking.Create(sessionId, userId, DateTime.UtcNow);

        Db.TrainingSessionBookings.Add(booking);
        return await SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveAsync(Guid sessionId, string userId)
    {
        var booking = await Db.TrainingSessionBookings
            .FirstOrDefaultAsync(x => x.TrainingSessionId == sessionId && x.UserId == userId);

        if (booking is null)
            return false;

        Db.TrainingSessionBookings.Remove(booking);
        return await SaveChangesAsync() > 0;
    }

    public async Task<IReadOnlySet<Guid>> GetBookedSessionIdsAsync(string userId)
    {
        var ids = await Db.TrainingSessionBookings
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.TrainingSessionId)
            .ToListAsync();

        return ids.ToHashSet();
    }
}