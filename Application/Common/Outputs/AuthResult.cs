
using Application.Common.Outputs;

namespace Application.Common.Outputs;

// Record för att representera resultatet av en autentiseringsoperation, används i IAuthService och IdentityAuthService
public sealed record AuthResult(bool Succeeded, AuthErrorType? ErrorType = null, string? ErrorMessage = null)
{
    public static AuthResult Ok() => new(true);
    public static AuthResult Failed(string? errorMessage = null) => new(false, AuthErrorType.Error, errorMessage);
    public static AuthResult InvalidCredentials() => new(false, AuthErrorType.InvalidCredentials);
    public static AuthResult RequireTwoFactorAuth() => new(false, AuthErrorType.RequireTwoFactorAuth);
    public static AuthResult LockedOut() => new(false, AuthErrorType.LockedOut);
    public static AuthResult NotAllowed() => new(false, AuthErrorType.NotAllowed);

};
