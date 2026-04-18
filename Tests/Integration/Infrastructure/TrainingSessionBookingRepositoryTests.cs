using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Infrastructure;

public class TrainingSessionBookingRepositoryTests
{
    private static PersistenceContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PersistenceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PersistenceContext(options);
    }

    [Fact]
    public async Task AddAsync_And_GetBookedSessionIdsAsync_Work()
    {
        await using var context = CreateContext();
        var repo = new TrainingSessionBookingRepository(context);

        var sessionId = Guid.NewGuid();
        var userId = "user1";

        var added = await repo.AddAsync(sessionId, userId);
        var ids = await repo.GetBookedSessionIdsAsync(userId);

        Assert.True(added);
        Assert.Contains(sessionId, ids);
    }

    [Fact]
    public async Task RemoveAsync_ReturnsFalse_WhenMissing()
    {
        await using var context = CreateContext();
        var repo = new TrainingSessionBookingRepository(context);

        var result = await repo.RemoveAsync(Guid.NewGuid(), "user1");

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveAsync_Removes_WhenExists()
    {
        await using var context = CreateContext();
        var repo = new TrainingSessionBookingRepository(context);

        var sessionId = Guid.NewGuid();
        var userId = "user1";

        await repo.AddAsync(sessionId, userId);
        var removed = await repo.RemoveAsync(sessionId, userId);
        var ids = await repo.GetBookedSessionIdsAsync(userId);

        Assert.True(removed);
        Assert.DoesNotContain(sessionId, ids);
    }
}