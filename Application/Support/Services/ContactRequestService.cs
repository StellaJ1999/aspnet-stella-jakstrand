
using Application.Abstractions.Support;
using Application.Support.Inputs;
using Domain.Support;

namespace Application.Support.Services;

//Service som hanterar kontaktförfrågningar. Denna service används av både MVC-controller och API-endpoint, och kan även användas i tester.
//Den är helt oberoende av hur data lagras (Repository) och hur den exponeras (Controller/Endpoint).

public sealed class ContactRequestService(IContactRequestRepository repo) : IContactRequestService
{
    public async Task<bool> CreateContactRequestAsync(ContactRequestInput input)
    {
        ArgumentNullException.ThrowIfNull(input);


        // Applikationslagret sätter Id och datum så att detta blir konsekvent oavsett om anropet kommer
        // från en MVC-controller, en API-endpoint eller ett test. Infrastructure ska bara spara.
        input.SetId(Guid.NewGuid().ToString());
        input.SetDate(DateTime.UtcNow);

        // Repository är en abstraktion som Infrastructure implementerar med EF Core (DbContext + SaveChanges).
        var result = await repo.AddAsync(input);
        return result;
    }

    // Här hämtar vi alla kontaktförfrågningar. Endast för administratörer.
    public async Task<IReadOnlyList<ContactRequest>> GetContactRequestAsync()
        => await repo.GetAllAsync();

}
