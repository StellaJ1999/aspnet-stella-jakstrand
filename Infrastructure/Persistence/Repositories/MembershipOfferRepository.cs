using Application.Abstractions.Memberships;
using Domain.Memberships;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class MembershipOfferRepository(PersistenceContext db)
    : RepositoryBase(db), IMembershipOfferRepository
{
    public async Task<IReadOnlyList<MembershipOffer>> GetAllAsync()
        => await Db.MembershipOffers.AsNoTracking().OrderBy(x => x.Tier).ToListAsync();

    public Task<MembershipOffer?> GetByTierAsync(MembershipTier tier)
        => Db.MembershipOffers.AsNoTracking().FirstOrDefaultAsync(x => x.Tier == tier);
}   