using Application.Abstractions.Support;
using Application.Extensions;
using Application.Support.Services;
using Infrastructure;
using Presentation.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApplication(builder.Configuration, builder.Environment);
builder.Services.AddSession();

builder.Services.AddScoped<IFaqService, FaqService>();
builder.Services.AddScoped<IContactRequestService, ContactRequestService>();


builder.Services.AddControllersWithViews();
builder.Services.AddRouting(x => x.LowercaseUrls = true);

var app = builder.Build();


app.UseHsts();
app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


await InfrastructureInitializer.InitializeAsync(app.Services, app.Environment, app.Configuration);

app.Run();
