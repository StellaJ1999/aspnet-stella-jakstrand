using Application.Abstractions.Training;
using Application.Training.Services;
using Domain.Common;
using Domain.Training;
using Moq;

namespace Tests.Unit.Application.Training;

public class TrainingSessionBookingServiceTests
{
    private static TrainingSession CreateSession(int maxParticipants = 2)
    {
        var start = DateTime.UtcNow.AddDays(1);
        var end = start.AddHours(1);

        return TrainingSession.Create(
            id: Guid.NewGuid(),
            name: "Yoga",
            startTime: start,
            endTime: end,
            maxParticipants: maxParticipants,
            createdUtc: DateTime.UtcNow);
    }

    [Fact]
    public async Task BookAsync_Throws_WhenUserIdMissing()
    {
        var sessionsRepo = new Mock<ITrainingSessionRepository>();
        var bookingsRepo = new Mock<ITrainingSessionBookingRepository>();
        var service = new TrainingSessionBookingService(sessionsRepo.Object, bookingsRepo.Object);

        await Assert.ThrowsAsync<DomainException>(() => service.BookAsync(Guid.NewGuid(), ""));
    }

    [Fact]
    public async Task BookAsync_ReturnsFalse_WhenSessionNotFound()
    {
        var sessionsRepo = new Mock<ITrainingSessionRepository>();
        var bookingsRepo = new Mock<ITrainingSessionBookingRepository>();
        sessionsRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((TrainingSession?)null);

        var service = new TrainingSessionBookingService(sessionsRepo.Object, bookingsRepo.Object);

        var result = await service.BookAsync(Guid.NewGuid(), "user1");

        Assert.False(result);
    }

    [Fact]
    public async Task BookAsync_ReturnsFalse_WhenAlreadyBooked()
    {
        var session = CreateSession();
        session.Bookings.Add(TrainingSessionBooking.Create(session.Id, "user1", DateTime.UtcNow));

        var sessionsRepo = new Mock<ITrainingSessionRepository>();
        var bookingsRepo = new Mock<ITrainingSessionBookingRepository>();
        sessionsRepo.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);

        var service = new TrainingSessionBookingService(sessionsRepo.Object, bookingsRepo.Object);

        var result = await service.BookAsync(session.Id, "user1");

        Assert.False(result);
    }

    [Fact]
    public async Task BookAsync_ReturnsFalse_WhenFull()
    {
        var session = CreateSession(maxParticipants: 1);
        session.Bookings.Add(TrainingSessionBooking.Create(session.Id, "other", DateTime.UtcNow));

        var sessionsRepo = new Mock<ITrainingSessionRepository>();
        var bookingsRepo = new Mock<ITrainingSessionBookingRepository>();
        sessionsRepo.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);

        var service = new TrainingSessionBookingService(sessionsRepo.Object, bookingsRepo.Object);

        var result = await service.BookAsync(session.Id, "user1");

        Assert.False(result);
    }

    [Fact]
    public async Task BookAsync_CallsRepo_WhenAvailable()
    {
        var session = CreateSession(maxParticipants: 5);

        var sessionsRepo = new Mock<ITrainingSessionRepository>();
        var bookingsRepo = new Mock<ITrainingSessionBookingRepository>();
        sessionsRepo.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        bookingsRepo.Setup(r => r.AddAsync(session.Id, "user1")).ReturnsAsync(true);

        var service = new TrainingSessionBookingService(sessionsRepo.Object, bookingsRepo.Object);

        var result = await service.BookAsync(session.Id, "user1");

        Assert.True(result);
        bookingsRepo.Verify(r => r.AddAsync(session.Id, "user1"), Times.Once);
    }

    [Fact]
    public async Task CancelBookingAsync_Throws_WhenUserIdMissing()
    {
        var sessionsRepo = new Mock<ITrainingSessionRepository>();
        var bookingsRepo = new Mock<ITrainingSessionBookingRepository>();
        var service = new TrainingSessionBookingService(sessionsRepo.Object, bookingsRepo.Object);

        await Assert.ThrowsAsync<DomainException>(() => service.CancelBookingAsync(Guid.NewGuid(), " "));
    }

    [Fact]
    public async Task GetBookingsForUserAsync_Throws_WhenUserIdMissing()
    {
        var sessionsRepo = new Mock<ITrainingSessionRepository>();
        var bookingsRepo = new Mock<ITrainingSessionBookingRepository>();
        var service = new TrainingSessionBookingService(sessionsRepo.Object, bookingsRepo.Object);

        await Assert.ThrowsAsync<DomainException>(() => service.GetBookingsForUserAsync(null!));
    }
}