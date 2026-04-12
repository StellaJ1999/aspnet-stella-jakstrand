using Application.Abstractions.Memberships;
using Domain.Memberships;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class MembershipOfferRepository(PersistenceContext db) : IMembershipOfferRepository
{
    public async Task<IReadOnlyList<MembershipOffer>> GetAllAsync()
        => await db.MembershipOffers.AsNoTracking().OrderBy(x => x.Tier).ToListAsync();

    public Task<MembershipOffer?> GetByTierAsync(MembershipTier tier)
        => db.MembershipOffers.AsNoTracking().FirstOrDefaultAsync(x => x.Tier == tier);
}