namespace Application.Support.Inputs;

public sealed record ContactRequestInput
(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Message
)
{
    public string Id { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    public void SetId(string id) => Id = id;
    public void SetDate(DateTime createdAt) => CreatedAt = createdAt;
}