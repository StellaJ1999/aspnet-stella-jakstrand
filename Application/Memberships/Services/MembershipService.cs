using Application.Abstractions.Memberships;
using Domain.Common;
using Domain.Memberships;

namespace Application.Memberships.Services;

internal sealed class MembershipService(
    IMembershipOfferRepository offersRepo,
    IMembershipSubscriptionRepository subsRepo) : IMembershipService
{
    public Task<IReadOnlyList<MembershipOffer>> GetOffersAsync()
        => offersRepo.GetAllAsync();

    public Task<MembershipSubscription?> GetActiveSubscriptionForUserAsync(string userId, DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("UserId is required.");

        return subsRepo.GetActiveForUserAsync(userId, utcNow);
    }

    public async Task<bool> CreateSubscriptionAsync(string userId, MembershipTier tier, int durationInMonths, DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("UserId is required.");

        var offer = await offersRepo.GetByTierAsync(tier);
        if (offer is null)
            return false;

        var existing = await subsRepo.GetActiveForUserAsync(userId, utcNow);
        if (existing is not null)
            return false;

        var subscription = MembershipSubscription.Create(
            id: default,
            userId: userId,
            membershipOfferId: offer.Id,
            durationInMonths: durationInMonths,
            startUtc: utcNow,
            createdUtc: utcNow);

        return await subsRepo.AddAsync(subscription);
    }

    public async Task<bool> CancelActiveSubscriptionAsync(string userId, DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("UserId is required.");

        var active = await subsRepo.GetActiveForUserAsync(userId, utcNow);
        if (active is null)
            return false;

        active.Cancel(utcNow);
        return await subsRepo.SaveChangesAsync();
    }
}