using Application.Training.Inputs;
using Domain.Training;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Infrastructure;

public class TrainingSessionRepositoryTests
{
    private static PersistenceContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PersistenceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PersistenceContext(options);
    }

    [Fact]
    public async Task CreateTrainingSessionAsync_AddsSession()
    {
        await using var context = CreateContext();
        var repo = new TrainingSessionRepository(context);

        var input = new TrainingSessionInput(
            Name: "Yoga",
            StartTime: DateTime.UtcNow.AddHours(1),
            EndTime: DateTime.UtcNow.AddHours(2),
            MaxParticipants: 10);

        input.SetId(Guid.NewGuid());
        input.SetCreatedUtc(DateTime.UtcNow);

        var result = await repo.CreateTrainingSessionAsync(input);

        Assert.True(result);
        Assert.Equal(1, await context.TrainingSessions.CountAsync());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOrderedByStartTime()
    {
        await using var context = CreateContext();

        var a = TrainingSession.Create(Guid.NewGuid(), "A",
            DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(3), 10, DateTime.UtcNow);
        var b = TrainingSession.Create(Guid.NewGuid(), "B",
            DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), 10, DateTime.UtcNow);

        context.TrainingSessions.AddRange(a, b);
        await context.SaveChangesAsync();

        var repo = new TrainingSessionRepository(context);
        var list = await repo.GetAllAsync();

        Assert.Equal("B", list[0].Name);
        Assert.Equal("A", list[1].Name);
    }

    [Fact]
    public async Task UpdateTrainingSessionAsync_ReturnsFalse_WhenMissing()
    {
        await using var context = CreateContext();
        var repo = new TrainingSessionRepository(context);

        var input = new TrainingSessionInput(
            Name: "Spin",
            StartTime: DateTime.UtcNow.AddHours(1),
            EndTime: DateTime.UtcNow.AddHours(2),
            MaxParticipants: 12);

        input.SetId(Guid.NewGuid());

        var result = await repo.UpdateTrainingSessionAsync(input);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTrainingSessionAsync_ReturnsFalse_WhenMissing()
    {
        await using var context = CreateContext();
        var repo = new TrainingSessionRepository(context);

        var result = await repo.DeleteTrainingSessionAsync(Guid.NewGuid());

        Assert.False(result);
    }
}