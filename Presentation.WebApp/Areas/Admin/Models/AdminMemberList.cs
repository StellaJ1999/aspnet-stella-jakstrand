namespace Presentation.WebApp.Areas.Admin.Models;

public sealed record AdminMembersListViewModel(IReadOnlyList<AdminMemberListItemViewModel> Members);