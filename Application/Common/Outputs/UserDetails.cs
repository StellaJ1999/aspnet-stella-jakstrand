namespace Application.Common.Outputs;

//Dto för user details, används i IUserService och IdentityAuthService
public sealed record UserDetails
(
    string UserId,
    string? Email,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? ImageUrl
);
