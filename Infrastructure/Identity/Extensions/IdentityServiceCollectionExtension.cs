using Application.Abstractions.Authentication;
using Infrastructure.Identity.Services;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Identity.Extensions;

public static class IdentityServiceCollectionExtension
{
    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        services.AddIdentity<AppUser, AppRole>(x =>
        {

            x.SignIn.RequireConfirmedAccount = false;
            x.Password.RequiredLength = 8;
            x.User.RequireUniqueEmail = true;

        }).AddEntityFrameworkStores<PersistenceContext>().AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(x =>
        {
            var loginPath = configuration?.GetValue<string>("CookieSetting:LoginPath") ?? "/sign-in";
            var logoutPath = configuration?.GetValue<string>("CookieSetting:LogoutPath") ?? "/sign-out";
            var accessDeniedPath = configuration?.GetValue<string>("CookieSetting:AccessDeniedPath") ?? "/access-denied";
            var cookieName = configuration?.GetValue<string>("CookieSetting:CookieName") ?? "MyAppCookie";
            var MaxAgeInDays = configuration?.GetValue<int>("CookieSetting:MaxAgeInDays") ?? 90;
            var ExpiresInDays = configuration?.GetValue<int>("CookieSetting:ExpiresInDays") ?? 30;

            x.LoginPath = loginPath;
            x.LogoutPath = logoutPath;
            x.AccessDeniedPath = accessDeniedPath;
            x.Cookie.Name = cookieName;
            x.Cookie.IsEssential = true;
            x.Cookie.MaxAge = TimeSpan.FromDays(MaxAgeInDays);
            x.ExpireTimeSpan = TimeSpan.FromDays(ExpiresInDays);
        });

        services.AddScoped<IAuthService, IdentityAuthService>();
        return services;
    }
}
