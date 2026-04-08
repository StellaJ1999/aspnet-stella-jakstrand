using Domain.Support;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts;
//PersistenceContext är den klass som representerar vår databas i EF Core.
//Den ärver från IdentityDbContext för att inkludera Identity-tabellerna (t.ex. AspNetUsers, AspNetRoles, etc.).

public class PersistenceContext(DbContextOptions<PersistenceContext> options) 
    : IdentityDbContext<AppUser, IdentityRole, string>(options)
{
    // DbSet behövs för att EF Core ska förstå att ContactRequest ska bli en tabell i databasen.
    // Utan DbSet (och migration) kommer inget ContactRequests-table att skapas och inget kan sparas.
    public DbSet<ContactRequest> ContactRequests => Set<ContactRequest>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
       
        base.OnModelCreating(builder);

        // Laddar alla IEntityTypeConfiguration<T> i samma assembly, t.ex. ContactRequestConfiguration.
        // Det gör att schema-regler hamnar i Infrastructure istället för att "läcka" in i Domain.
        builder.ApplyConfigurationsFromAssembly(typeof(PersistenceContext).Assembly);
    }
}
