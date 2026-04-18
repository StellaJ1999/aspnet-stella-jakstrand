using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests.Unit.Identity;

internal static class IdentityTestHelpers
{
    public static Mock<UserManager<AppUser>> CreateUserManagerMock(Mock<IUserStore<AppUser>> store)
    {
        return new Mock<UserManager<AppUser>>(
            store.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            new PasswordHasher<AppUser>(),
            Array.Empty<IUserValidator<AppUser>>(),
            Array.Empty<IPasswordValidator<AppUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<AppUser>>>());
    }

    public static RoleManager<IdentityRole> CreateRoleManager(Mock<IRoleStore<IdentityRole>> store)
    {
        return new RoleManager<IdentityRole>(
            store.Object,
            Array.Empty<IRoleValidator<IdentityRole>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            Mock.Of<ILogger<RoleManager<IdentityRole>>>());
    }

    public static Mock<SignInManager<AppUser>> CreateSignInManagerMock(UserManager<AppUser> userManager)
    {
        return new Mock<SignInManager<AppUser>>(
            userManager,
            new HttpContextAccessor(),
            new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<ILogger<SignInManager<AppUser>>>(),
            Mock.Of<IAuthenticationSchemeProvider>(),
            Mock.Of<IUserConfirmation<AppUser>>());
    }
}