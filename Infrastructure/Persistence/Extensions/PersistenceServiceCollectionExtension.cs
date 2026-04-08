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

        var connectionString = configuration.GetConnectionString("SqlConnection")
            ?? throw new InvalidOperationException("Connection string 'SqlConnection' was not found.");

        if (environment.IsDevelopment())
        {
            services.AddDbContext<PersistenceContext>(x => x.UseSqlite(connectionString));
        }
        else
        {
            services.AddDbContext<PersistenceContext>(x => x.UseSqlServer(connectionString));
        }

        return services;
    }
}
