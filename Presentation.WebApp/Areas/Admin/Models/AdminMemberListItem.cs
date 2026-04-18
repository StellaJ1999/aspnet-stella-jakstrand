namespace Presentation.WebApp.Areas.Admin.Models;

public sealed record AdminMemberListItemViewModel(
    string Id,
    string Email,
    string? FirstName,
    string? LastName,
    bool IsAdmin,
    bool IsMember);