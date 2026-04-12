using Application.Abstractions.Memberships;
using Domain.Memberships;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class MembershipSubscriptionRepository(PersistenceContext db)
    : RepositoryBase(db), IMembershipSubscriptionRepository
{
    public Task<MembershipSubscription?> GetActiveForUserAsync(string userId, DateTime utcNow)
        => Db.MembershipSubscriptions.FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.Status == MembershipSubscriptionStatus.Active &&
            x.EndUtc > utcNow);

    public async Task<bool> AddAsync(MembershipSubscription subscription)
    {
        Db.MembershipSubscriptions.Add(subscription);
        return await base.SaveChangesAsync() > 0;
    }

    public async Task<bool> SaveChangesAsync()
        => await base.SaveChangesAsync() > 0;
}   