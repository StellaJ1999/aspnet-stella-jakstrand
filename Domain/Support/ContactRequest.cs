using Domain.Common;

namespace Domain.Support;

public class ContactRequest
{
    private ContactRequest() { }

    public Guid Id { get; private set; }

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Phone { get; private set; }
    public string Message { get; private set; } = null!;

    public DateTime CreatedUtc { get; private set; } = DateTime.UtcNow;

    public static ContactRequest Create(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string? phone,
        string message,
        DateTime createdUtc)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        if (string.IsNullOrWhiteSpace(message))
            throw new DomainException("Message is required.");

        return new ContactRequest
        {
            Id = id == default ? Guid.NewGuid() : id,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim(),
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim(),
            Message = message.Trim(),
            CreatedUtc = createdUtc == default ? DateTime.UtcNow : createdUtc,
        };
    }
}
