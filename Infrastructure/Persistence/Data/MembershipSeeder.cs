using Domain.Memberships;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data;

public static class MembershipSeeder
{

    private static readonly Guid StandardOfferId = Guid.Parse("d94c1f8a-3d1f-4f7f-8d73-8f6f8b0a6b12");
    private static readonly Guid PremiumOfferId = Guid.Parse("2f7c4c1d-9c2f-4c74-9f8c-cc3b6f0f3f4a");

    public static async Task SeedAsync(PersistenceContext db)
    {
        if (await db.MembershipOffers.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        db.MembershipOffers.Add(MembershipOffer.Create(StandardOfferId, MembershipTier.Standard, 495m, now));
        db.MembershipOffers.Add(MembershipOffer.Create(PremiumOfferId, MembershipTier.Premium, 595m, now));

        await db.SaveChangesAsync();
    }
}
