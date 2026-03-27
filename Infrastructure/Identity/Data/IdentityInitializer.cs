
using Application.Abstractions.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity.Data;

internal class IdentityInitializer
{
    public static async Task AddDefaultAdmin(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var authService= scope.ServiceProvider.GetRequiredService<IAuthService>();

        var result = await authService.SignUpUserAsync("admin@localhost.com", "BytMig123!");


    }
}
