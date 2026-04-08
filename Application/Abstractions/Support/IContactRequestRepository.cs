using Application.Support.Inputs;
using Domain.Support;

namespace Application.Abstractions.Support;

public interface IContactRequestRepository
{
    Task<bool> AddAsync(ContactRequestInput model);
    Task<IReadOnlyList<ContactRequest>> GetAllAsync();
}
