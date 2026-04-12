using Domain.Memberships;

namespace Application.Abstractions.Memberships;

public interface IMembershipOfferRepository
{
    Task<IReadOnlyList<MembershipOffer>> GetAllAsync();
    Task<MembershipOffer?> GetByTierAsync(MembershipTier tier);
}