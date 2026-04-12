using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence.Repositories.Base;

public abstract class RepositoryBase(PersistenceContext db)
{
    protected PersistenceContext Db { get; } = db ?? throw new ArgumentNullException(nameof(db));

    protected Task<int> SaveChangesAsync(CancellationToken ct = default)
        => Db.SaveChangesAsync(ct);
}