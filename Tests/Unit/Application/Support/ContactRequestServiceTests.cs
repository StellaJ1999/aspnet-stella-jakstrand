using Application.Abstractions.Support;
using Application.Support.Inputs;
using Application.Support.Services;
using Moq;

namespace Tests.Unit.Application.Support;

public class ContactRequestServiceTests
{
    [Fact]
    public async Task CreateContactRequestAsync_SetsIdAndDate_AndCallsRepo()
    {
        var repo = new Mock<IContactRequestRepository>();
        ContactRequestInput? captured = null;

        repo.Setup(r => r.AddAsync(It.IsAny<ContactRequestInput>()))
            .Callback<ContactRequestInput>(input => captured = input)
            .ReturnsAsync(true);

        var service = new ContactRequestService(repo.Object);
        var input = new ContactRequestInput(
            FirstName: "Stella",
            LastName: "J",
            Email: "stella@example.com",
            Phone: "0701234567",
            Message: "Hello");

        var result = await service.CreateContactRequestAsync(input);

        Assert.True(result);
        Assert.NotNull(captured);
        Assert.False(string.IsNullOrWhiteSpace(captured!.Id));
        Assert.True(captured.CreatedAt > DateTime.MinValue);
    }
}