using Application.Common.Outputs;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Tests.Unit.Identity;

public partial class IdentityUserServiceTests
{
    [Fact]
    public async Task GetUserDetailsAsync_ReturnsNotFound_WhenUserMissing()
    {
        var store = new Mock<IUserStore<AppUser>>();
        var userManagerMock = IdentityTestHelpers.CreateUserManagerMock(store);
        var userManager = userManagerMock.Object;

        userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync((AppUser?)null);

        var service = new IdentityUserService(userManager);

        var result = await service.GetUserDetailsAsync("1");

        Assert.False(result.Succeeded);
        Assert.Equal(UserErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task UpdateUserDetailsAsync_ReturnsFailed_WhenUpdateFails()
    {
        var store = new Mock<IUserStore<AppUser>>();
        var userManagerMock = IdentityTestHelpers.CreateUserManagerMock(store);
        var userManager = userManagerMock.Object;

        var user = new AppUser { Id = "1" };
        userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);
        userManagerMock.Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "fail" }));

        var service = new IdentityUserService(userManager);

        var result = await service.UpdateUserDetailsAsync(new UserDetails("1", "a@b.com", "A", "B", "1", null));

        Assert.False(result.Succeeded);
        Assert.Equal(UserErrorType.Error, result.ErrorType);
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsNotFound_WhenUserMissing()
    {
        var store = new Mock<IUserStore<AppUser>>();
        var userManagerMock = IdentityTestHelpers.CreateUserManagerMock(store);
        var userManager = userManagerMock.Object;

        userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync((AppUser?)null);

        var service = new IdentityUserService(userManager);

        var result = await service.DeleteUserAsync("1");

        Assert.False(result.Succeeded);
        Assert.Equal(UserErrorType.NotFound, result.ErrorType);
    }
}