namespace Application.Common.Outputs;

public sealed record UserResult(bool Succeeded, string? UserId = null, UserDetails? Details = null, UserErrorType? ErrorType = null, string? ErrorMessage = null)
{
    public static UserResult Ok() => new(true);
    public static UserResult Ok(string userId) => new(true, userId);
    public static UserResult Ok(string userId, UserDetails userDetails) => new(true, userId, userDetails);

    public static UserResult NotFound() => new(false, ErrorType: UserErrorType.NotFound);
    public static UserResult Failed() => new(false, ErrorType: UserErrorType.Error);
    public static UserResult Failed(string errorMessage) => new(false, ErrorType: UserErrorType.Error, ErrorMessage: errorMessage);
}