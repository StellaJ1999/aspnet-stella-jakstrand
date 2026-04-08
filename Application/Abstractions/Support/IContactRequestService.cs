using Application.Support.Inputs;
using Domain.Support;

namespace Application.Abstractions.Support;

public interface IContactRequestService
{
    Task<bool> CreateContactRequestAsync(ContactRequestInput input);

    // För administratörer att kunna hämta alla kontaktförfrågningar
    Task<IReadOnlyList<ContactRequest>> GetContactRequestAsync();
}
