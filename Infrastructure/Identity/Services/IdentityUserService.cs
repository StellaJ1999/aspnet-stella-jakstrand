using Application.Abstractions.Identity;
using Application.Common.Outputs;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Services;

public class IdentityUserService(UserManager<AppUser> userManager) : IUserService
{
    public async Task<UserResult> GetUserDetailsAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return UserResult.NotFound();

        var userDetails = new UserDetails
        (
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.ImageUrl
        );

        return UserResult.Ok(user.Id, userDetails);
    }

    public async Task<UserResult> UpdateUserDetailsAsync(UserDetails userDetails)
    {
        ArgumentNullException.ThrowIfNull(userDetails);

        var user = await userManager.FindByIdAsync(userDetails.UserId);
        if (user is null)
            return UserResult.NotFound();

        user.FirstName = userDetails.FirstName;
        user.LastName = userDetails.LastName;
        user.PhoneNumber = userDetails.PhoneNumber;
        user.ImageUrl = userDetails.ImageUrl;

        var result = await userManager.UpdateAsync(user);
        return result.Succeeded
            ? UserResult.Ok()
            : UserResult.Failed(result.Errors.FirstOrDefault()?.Description ?? "Unable to save changes");
    }
}
