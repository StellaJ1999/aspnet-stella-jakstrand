using Application.Abstractions.Memberships;
using Application.Abstractions.Support;
using Application.Abstractions.Training;
using Application.Memberships.Services;
using Application.Support.Services;
using Application.Training.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Extensions;

public static class ApplicationServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        services.AddScoped<IContactRequestService, ContactRequestService>();
        services.AddScoped<ITrainingSessionService, TrainingSessionService>();
        services.AddScoped<ITrainingSessionBookingService, TrainingSessionBookingService>();
        services.AddScoped<IMembershipService, MembershipService>();

        return services;
    }
}
