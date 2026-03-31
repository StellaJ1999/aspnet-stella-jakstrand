using Infrastructure.Identity.Data;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// InfrastructureInitializer körs vid applikationsstart (innan appen börjar ta emot HTTP-requests).
namespace Infrastructure;

public class InfrastructureInitializer
{
    public static async Task InitializeAsync(IServiceProvider sp, IHostEnvironment env, IConfiguration cfg)
    {
        // Vi skapar ett DI-scope manuellt vid startup.
        // En scoped service betyder att samma instans används inom ett scope (oftast en HTTP-request),
        // och när scopet avslutas så disposas instansen och resurser (t.ex. DB-anslutning) städas bort.
        // Vid startup finns ingen HTTP-request som skapar ett scope automatiskt, därför gör vi det här.

        await using var scope = sp.CreateAsyncScope();


        // Kör EF migrations mot databasen för aktuell miljö (skapar/uppdaterar tabeller, t.ex. Identity-tabeller).
        var db = scope.ServiceProvider.GetRequiredService<PersistenceContext>();

        // Om databasen är relationsdatabas (t.ex. SQL Server, PostgreSQL) så kör vi EF migrations.
        //Annars (t.ex. SQLite i minnet) så skapar vi databasen direkt från EF-modellerna.
        if (db.Database.IsRelational())
        {
            await db.Database.MigrateAsync();
        }
        else
        {
            await db.Database.EnsureCreatedAsync();
        }
        // Seeda bara i utvecklingsmiljö, eller om det är uttryckligen aktiverat i konfigurationen

        var seedDefaultAdmin = cfg.GetValue<bool>("Seed:DefaultAdmin");
        if (env.IsDevelopment() && seedDefaultAdmin)
        {
            await IdentityInitializer.AddDefaultAdmin(scope.ServiceProvider);
        }
    }
}
