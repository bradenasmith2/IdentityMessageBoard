using IdentityMessageBoard.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IdentityMessageBoard.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MessageBoardContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityMessageBoardDb")).UseSnakeCaseNamingConvention());

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MessageBoardContext>();

void ConfigureServices(IServiceCollection services)
{
    // ...

    services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<MessageBoardContext>();

    // ...

    using (var scope = services.BuildServiceProvider().CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new List<string> { "Admin", "User", "NonUser" };
        foreach (var roleName in roles)
        {
            if (!roleManager.RoleExistsAsync(roleName).Result)
            {
                var role = new IdentityRole { Name = roleName };
                roleManager.CreateAsync(role).Wait();
            }
        }
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
