
using Application.Common.Results;

namespace Application.Abstractions.Authentication;

public interface IAuthService
{
    Task<AuthResult> SignUpUserAsync(string email, string password, string? roleName = null);
    Task<AuthResult> SignInUserAsync(string email, string password, bool rememberMe);
    Task SignOutUserAsync();
}
