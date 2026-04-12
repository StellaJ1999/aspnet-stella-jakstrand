using Application.Abstractions.Support;
using Application.Support.Inputs;
using Domain.Support;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class ContactRequestRepository(PersistenceContext db)
    : RepositoryBase(db), IContactRequestRepository
{
    public async Task<bool> AddAsync(ContactRequestInput model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var id = Guid.TryParse(model.Id, out var guid) ? guid : Guid.NewGuid();
        var createdUtc = model.CreatedAt == default ? DateTime.UtcNow : model.CreatedAt;

        var entity = ContactRequest.Create(
            id: id,
            firstName: model.FirstName,
            lastName: model.LastName,
            email: model.Email,
            phone: model.Phone,
            message: model.Message,
            createdUtc: createdUtc
        );

        Db.ContactRequests.Add(entity);

        var result = await SaveChangesAsync();
        return result > 0;
    }

    public async Task<IReadOnlyList<ContactRequest>> GetAllAsync()
    {
        return await Db.ContactRequests
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedUtc)
            .ToListAsync();
    }
}