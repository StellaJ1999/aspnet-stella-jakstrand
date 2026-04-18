using Application.Support.Inputs;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Infrastructure;

public class ContactRequestRepositoryTests
{
    private static PersistenceContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PersistenceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PersistenceContext(options);
    }

    [Fact]
    public async Task AddAsync_PersistsContactRequest()
    {
        await using var context = CreateContext();
        var repo = new ContactRequestRepository(context);

        var input = new ContactRequestInput("A", "B", "a@b.com", null, "msg");
        input.SetId(Guid.NewGuid().ToString());
        input.SetDate(DateTime.UtcNow);

        var result = await repo.AddAsync(input);

        Assert.True(result);
        Assert.Equal(1, await context.ContactRequests.CountAsync());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsNewestFirst()
    {
        await using var context = CreateContext();
        var repo = new ContactRequestRepository(context);

        var older = new ContactRequestInput("A", "B", "a@b.com", null, "old");
        older.SetId(Guid.NewGuid().ToString());
        older.SetDate(DateTime.UtcNow.AddDays(-1));

        var newer = new ContactRequestInput("C", "D", "c@d.com", null, "new");
        newer.SetId(Guid.NewGuid().ToString());
        newer.SetDate(DateTime.UtcNow);

        await repo.AddAsync(older);
        await repo.AddAsync(newer);

        var result = await repo.GetAllAsync();

        Assert.Equal("new", result[0].Message);
        Assert.Equal("old", result[1].Message);
    }
}