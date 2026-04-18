using Application.Abstractions.Training;
using Application.Training.Inputs;
using Application.Training.Services;
using Moq;

namespace Tests.Unit.Application.Training;

public class TrainingSessionServiceTests
{
    [Fact]
    public async Task CreateTrainingSessionAsync_SetsIdAndCreatedUtc_AndCallsRepo()
    {
        var repo = new Mock<ITrainingSessionRepository>();
        TrainingSessionInput? captured = null;

        repo.Setup(r => r.CreateTrainingSessionAsync(It.IsAny<TrainingSessionInput>()))
            .Callback<TrainingSessionInput>(input => captured = input)
            .ReturnsAsync(true);

        var service = new TrainingSessionService(repo.Object);
        var input = new TrainingSessionInput(
            Name: "Yoga",
            StartTime: DateTime.UtcNow.AddHours(1),
            EndTime: DateTime.UtcNow.AddHours(2),
            MaxParticipants: 10);

        var result = await service.CreateTrainingSessionAsync(input);

        Assert.True(result);
        Assert.NotNull(captured);
        Assert.NotEqual(Guid.Empty, captured!.Id);
        Assert.True(captured.CreatedUtc > DateTime.MinValue);
    }

    [Fact]
    public async Task UpdateTrainingSessionAsync_SetsIdBeforeRepoCall()
    {
        var repo = new Mock<ITrainingSessionRepository>();
        TrainingSessionInput? captured = null;

        repo.Setup(r => r.UpdateTrainingSessionAsync(It.IsAny<TrainingSessionInput>()))
            .Callback<TrainingSessionInput>(input => captured = input)
            .ReturnsAsync(true);

        var service = new TrainingSessionService(repo.Object);
        var id = Guid.NewGuid();

        var input = new TrainingSessionInput(
            Name: "Spin",
            StartTime: DateTime.UtcNow.AddHours(3),
            EndTime: DateTime.UtcNow.AddHours(4),
            MaxParticipants: 12);

        var result = await service.UpdateTrainingSessionAsync(id, input);

        Assert.True(result);
        Assert.NotNull(captured);
        Assert.Equal(id, captured!.Id);
    }
}