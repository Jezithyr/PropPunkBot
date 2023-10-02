using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropPunkShared;
using PropPunkShared.Core;
using PropPunkShared.Database;
using PropPunkShared.Services;

var builder = WebApplication.CreateBuilder(args);

Env.EnsureLoadEnvFile();

// Add services to the container.
var connectionString = Env.CreateConnectionString() ??
                       throw new InvalidOperationException("Connection string not found.");
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("PropPunkUniverse")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//auto register service dependencies
ServiceBase.AutoRegisterServices(
    builder.Services,
    typeof(ConfigService).Assembly,
    typeof(CountryService).Assembly);
builder.Services.AddScoped<ConfigSyncService>();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<DatabaseContext>();
builder.Services.AddRazorPages();
builder.Services.AddAuthentication().AddDiscord(options =>
{
    options.ClientId = Env.Get("DISCORD_ID");
    options.ClientSecret = Env.Get("DISCORD_SECRET");
    options.Scope.Clear();
    options.Scope.Add("identify guilds");
    options.SaveTokens = true;
    options.ClaimActions.MapJsonKey("discord_display_name","global_name", "string");
    options.ClaimActions.MapCustomJson("discord_profile_picture", user =>
        string.Format(
            CultureInfo.InvariantCulture,
            "https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
            user.GetString("id"),
            user.GetString("avatar"),
            user.GetString("avatar")!.StartsWith("a_") ? "gif" : "png"));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

await using (var scope = app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.MigrateAsync();
}
app.Services.GetService<ConfigService>()?.UpdateCachedSettings();

app.Run();
