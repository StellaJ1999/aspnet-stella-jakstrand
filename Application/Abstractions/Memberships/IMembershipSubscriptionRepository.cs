using System;
using System.Collections.Generic;
using System.Text;
using Domain.Memberships;

namespace Application.Abstractions.Memberships;

public interface IMembershipSubscriptionRepository
{
    Task<MembershipSubscription?> GetActiveForUserAsync(string userId, DateTime utcNow);
    Task<bool> AddAsync(MembershipSubscription subscription);
    Task<bool> SaveChangesAsync();
}
