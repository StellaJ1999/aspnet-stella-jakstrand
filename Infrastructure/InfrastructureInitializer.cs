using Infrastructure.Identity.Data;
using Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure;

public class InfrastructureInitializer
{
    public static async Task InitializeAsync(IServiceProvider sp, IHostEnvironment env, IConfiguration cfg)
    {
        await using var scope = sp.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<PersistenceContext>();
        await db.Database.MigrateAsync();

        // Seeda bara i utvecklingsmiljö, eller om det är uttryckligen aktiverat i konfigurationen

        var seedDefaultAdmin = cfg.GetValue<bool>("Seed:DefaultAdmin");
        if (env.IsDevelopment() && seedDefaultAdmin)
        {
            await IdentityInitializer.AddDefaultAdmin(scope.ServiceProvider);
        }
    }
}
