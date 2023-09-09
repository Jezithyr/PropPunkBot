using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropPunkShared;
using PropPunkShared.Database;

var builder = WebApplication.CreateBuilder(args);

Env.EnsureLoadEnvFile();

// Add services to the container.
var connectionString = Env.CreateConnectionString() ??
                       throw new InvalidOperationException("Connection string not found.");
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("PropPunkUniverse")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DatabaseContext>();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication().AddDiscord(options =>
{
    options.ClientId = Env.Get("DISCORD_ID");
    options.ClientSecret = Env.Get("DISCORD_SECRET");
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

app.Run();
