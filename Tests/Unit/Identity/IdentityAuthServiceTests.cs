using Application.Common.Outputs;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Tests.Unit.Identity;

public partial class IdentityAuthServiceTests
{
    [Fact]
    public async Task SignInUserAsync_ReturnsInvalidCredentials_WhenEmailMissing()
    {
        var userStore = new Mock<IUserStore<AppUser>>();
        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        var userManagerMock = IdentityTestHelpers.CreateUserManagerMock(userStore);
        var userManager = userManagerMock.Object;
        var roleManager = IdentityTestHelpers.CreateRoleManager(roleStore);
        var signInManagerMock = IdentityTestHelpers.CreateSignInManagerMock(userManager);
        var signInManager = signInManagerMock.Object;

        var service = new IdentityAuthService(userManager, signInManager, roleManager);

        var result = await service.SignInUserAsync("", "pass", false);

        Assert.False(result.Succeeded);
        Assert.Equal(AuthErrorType.InvalidCredentials, result.ErrorType);
    }

    [Fact]
    public async Task SignInUserAsync_ReturnsNotAllowed_WhenNotAllowed()
    {
        var userStore = new Mock<IUserStore<AppUser>>();
        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        var userManagerMock = IdentityTestHelpers.CreateUserManagerMock(userStore);
        var userManager = userManagerMock.Object;
        var roleManager = IdentityTestHelpers.CreateRoleManager(roleStore);
        var signInManagerMock = IdentityTestHelpers.CreateSignInManagerMock(userManager);
        var signInManager = signInManagerMock.Object;

        var user = new AppUser { Email = "a@b.com", UserName = "a@b.com" };
        userManagerMock.Setup(x => x.FindByEmailAsync("a@b.com")).ReturnsAsync(user);
        signInManagerMock.Setup(x => x.PasswordSignInAsync(user, "pass", false, false))
            .ReturnsAsync(SignInResult.NotAllowed);

        var service = new IdentityAuthService(userManager, signInManager, roleManager);

        var result = await service.SignInUserAsync("a@b.com", "pass", false);

        Assert.False(result.Succeeded);
        Assert.Equal(AuthErrorType.NotAllowed, result.ErrorType);
    }

    [Fact]
    public async Task SignInUserAsync_ReturnsRequireTwoFactor_WhenRequired()
    {
        var userStore = new Mock<IUserStore<AppUser>>();
        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        var userManagerMock = IdentityTestHelpers.CreateUserManagerMock(userStore);
        var userManager = userManagerMock.Object;
        var roleManager = IdentityTestHelpers.CreateRoleManager(roleStore);
        var signInManagerMock = IdentityTestHelpers.CreateSignInManagerMock(userManager);
        var signInManager = signInManagerMock.Object;

        var user = new AppUser { Email = "a@b.com", UserName = "a@b.com" };
        userManagerMock.Setup(x => x.FindByEmailAsync("a@b.com")).ReturnsAsync(user);
        signInManagerMock.Setup(x => x.PasswordSignInAsync(user, "pass", false, false))
            .ReturnsAsync(SignInResult.TwoFactorRequired);

        var service = new IdentityAuthService(userManager, signInManager, roleManager);

        var result = await service.SignInUserAsync("a@b.com", "pass", false);

        Assert.False(result.Succeeded);
        Assert.Equal(AuthErrorType.RequireTwoFactorAuth, result.ErrorType);
    }

    [Fact]
    public async Task SignInUserAsync_ReturnsFailed_WhenSignInFails()
    {
        var userStore = new Mock<IUserStore<AppUser>>();
        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        var userManagerMock = IdentityTestHelpers.CreateUserManagerMock(userStore);
        var userManager = userManagerMock.Object;
        var roleManager = IdentityTestHelpers.CreateRoleManager(roleStore);
        var signInManagerMock = IdentityTestHelpers.CreateSignInManagerMock(userManager);
        var signInManager = signInManagerMock.Object;

        var user = new AppUser { Email = "a@b.com", UserName = "a@b.com" };
        userManagerMock.Setup(x => x.FindByEmailAsync("a@b.com")).ReturnsAsync(user);
        signInManagerMock.Setup(x => x.PasswordSignInAsync(user, "pass", false, false))
            .ReturnsAsync(SignInResult.Failed);

        var service = new IdentityAuthService(userManager, signInManager, roleManager);

        var result = await service.SignInUserAsync("a@b.com", "pass", false);

        Assert.False(result.Succeeded);
        Assert.Equal(AuthErrorType.Error, result.ErrorType);
    }
}