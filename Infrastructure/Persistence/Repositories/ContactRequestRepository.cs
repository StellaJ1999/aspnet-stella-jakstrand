using Application.Abstractions.Support;
using Application.Support.Inputs;
using Domain.Support;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class ContactRequestRepository(PersistenceContext db) : IContactRequestRepository
{
    public async Task<bool> AddAsync(ContactRequestInput model)
    {
        // Repository är en gräns mot databasen och ska aldrig acceptera null som input.
        ArgumentNullException.ThrowIfNull(model);

        // `ContactRequestInput` kommer från Application och är en "use-case input".
        // För att spara i DB behöver vi en entitet som EF Core mappar till en tabell, här används `Domain.Support.ContactRequest`.
        // Det är normalt att Infrastructure känner till Domain eftersom Infrastructure är ett yttre lager.

        var id = Guid.TryParse(model.Id, out var guid) ? guid : Guid.NewGuid();
        var createdUtc = model.CreatedAt == default ? DateTime.UtcNow : model.CreatedAt;

        var entity = new ContactRequest
        {
            Id = id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            Message = model.Message,
            CreatedUtc = createdUtc
        };

        // Add lägger entiteten i EF Cores change tracker så att en INSERT genereras när vi sparar.
        db.ContactRequests.Add(entity);

        // SaveChangesAsync returnerar antalet ändrade rader i databasen, så vi kollar att det är mer än 0 för att veta att det lyckades.
        var result = await db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IReadOnlyList<ContactRequest>> GetAllAsync()
    {
        // AsNoTracking gör läsningen snabbare när du bara ska visa data (till exempel i en adminvy).
        // OrderByDescending gör att senaste ärenden visas först.
        return await db.ContactRequests
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedUtc)
            .ToListAsync();
    }
}
