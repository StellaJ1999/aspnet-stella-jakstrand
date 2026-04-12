using Application.Abstractions.Memberships;
using Domain.Memberships;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class MembershipSubscriptionRepository(PersistenceContext db) : IMembershipSubscriptionRepository
{
    public Task<MembershipSubscription?> GetActiveForUserAsync(string userId, DateTime utcNow)
        => db.MembershipSubscriptions.FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.Status == MembershipSubscriptionStatus.Active &&
            x.EndUtc > utcNow);

    public async Task<bool> AddAsync(MembershipSubscription subscription)
    {
        db.MembershipSubscriptions.Add(subscription);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> SaveChangesAsync()
        => await db.SaveChangesAsync() > 0;
}