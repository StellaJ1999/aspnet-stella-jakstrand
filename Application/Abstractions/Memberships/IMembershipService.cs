using Domain.Memberships;

namespace Application.Abstractions.Memberships;

public interface IMembershipService
{
    Task<IReadOnlyList<MembershipOffer>> GetOffersAsync();
    Task<MembershipSubscription?> GetActiveSubscriptionForUserAsync(string userId, DateTime utcNow);
    Task<bool> CreateSubscriptionAsync(string userId, MembershipTier tier, int durationInMonths, DateTime utcNow);
    Task<bool> CancelActiveSubscriptionAsync(string userId, DateTime utcNow);
}