namespace Application.Common.Outputs;
// Enum för att specificera olika typer av autentiseringsfel, används i IAuthService och IdentityAuthService
public enum AuthErrorType
{
    InvalidCredentials,
    RequireTwoFactorAuth,
    LockedOut,
    NotAllowed,
    Error
}