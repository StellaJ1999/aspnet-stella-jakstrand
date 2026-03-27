
namespace Application.Common.Results;

public sealed record AuthResult(bool Succeeded, string? ErrorMessage = null) 
{
    public static AuthResult Ok() => new(true);
    public static AuthResult Failed(string errorMessage) => new(false, errorMessage);

}
