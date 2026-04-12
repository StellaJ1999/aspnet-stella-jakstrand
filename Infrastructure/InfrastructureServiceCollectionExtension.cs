using Application.Abstractions.Memberships;
using Application.Abstractions.Support;
using Application.Abstractions.Training;
using Infrastructure.Identity.Extensions;
using Infrastructure.Persistence.Extensions;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class InfrastructureServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,  IConfiguration configuration,  IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        // Registrerar PersistenceContext och väljer provider, InMemory i dev om det är aktiverat annars SQL Server.
        services.AddPersistence(configuration, environment);

        // Registrerar ASP.NET Core Identity som använder samma DbContext.
        services.AddIdentity(configuration, environment);

        // Kopplar Application-kontraktet till Infrastructure-implementationen.
        // ContactRequestService anropar IContactRequestRepository och kommer via DI få ContactRequestRepository.
        services.AddScoped<IContactRequestRepository, ContactRequestRepository>();

        services.AddScoped<ITrainingSessionRepository, TrainingSessionRepository>();
        services.AddScoped<ITrainingSessionBookingRepository, TrainingSessionBookingRepository>();
        services.AddScoped<IMembershipOfferRepository, MembershipOfferRepository>();
        services.AddScoped<IMembershipSubscriptionRepository, MembershipSubscriptionRepository>();

        return services;
    }
}
