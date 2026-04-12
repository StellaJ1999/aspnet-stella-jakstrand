using Infrastructure.Identity.Data;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        // Om databasen inte finns så skapas den i utvecklingsmiljö, annars körs migreringar (om det inte redan är gjort).
        if (env.IsDevelopment())
        {
            await db.Database.EnsureCreatedAsync();
        }
        else
        {
            try
            {
                await db.Database.MigrateAsync();
            }
            catch { }
        }

        try
        {
            await MembershipSeeder.SeedAsync(db);
        }
        catch { }

        // Seeda bara i utvecklingsmiljö, eller om det är uttryckligen aktiverat i konfigurationen

        var seedDefaultAdmin = cfg.GetValue<bool>("Seed:DefaultAdmin");

        if (env.IsDevelopment() && seedDefaultAdmin)
        {
            // Seeda default roller och admin-konton i Identity-databasen.
            await IdentityInitializer.InitilizeDefaultRolesAsync(scope.ServiceProvider);
            await IdentityInitializer.InitilizeDefaultAdminAccountsAsync(scope.ServiceProvider);
        }

    }
}
