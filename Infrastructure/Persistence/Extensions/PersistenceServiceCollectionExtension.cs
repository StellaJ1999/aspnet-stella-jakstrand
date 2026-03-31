using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Persistence.Extensions;

// Denna extension-metod används i Program.cs för att registrera PersistenceContext i DI-containern,
// så att det kan injiceras i andra delar av applikationen (t.ex. IdentityServiceCollectionExtension, Repositories, etc.).
public static class PersistenceServiceCollectionExtension
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        var useInMemory = environment.IsDevelopment() && configuration.GetValue<bool>("Persistence:UseInMemory");

        if (useInMemory)
        {
            // Om vi använder in-memory databas så kan vi ge den ett namn (annars används ett slumpmässigt namn varje gång,
            // vilket gör att datan inte bevaras mellan olika körningar av applikationen).

            var dbName = configuration.GetValue<string>("Persistence:InMemoryDatabaseName") ?? "DymlecoDevelopementDB";
            services.AddDbContext<PersistenceContext>(x => x.UseInMemoryDatabase(dbName));
            return services;
        }

        var connectionString = configuration.GetConnectionString("SqlConnection")
            ?? throw new InvalidOperationException("Connection string 'SqlConnection' was not found.");

        services.AddDbContext<PersistenceContext>(x => x.UseSqlServer(connectionString));
        return services;
    }
}
