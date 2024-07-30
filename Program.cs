using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LeavePortal.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using AspNetCoreHero.ToastNotification;
using LeavePortal.Helpers;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var LoggerFactory = service.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = service.GetRequiredService<ApplicationDbContext>();

        if (context.Database.CanConnect())
        {
            Console.WriteLine("The database already exists.");
        }
        else
        {
            context.Database.Migrate();
        }

        var UserManeger = service.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManeger = service.GetRequiredService<RoleManager<IdentityRole>>();
        var contextservice = service.GetRequiredService<ApplicationDbContext>();

        //Add defafule Three roles
        await ContextSeed.seedRolesAsync(service.GetRequiredService<RoleManager<IdentityRole>>());

        //Add default Admin who manage web application

        await ContextSeed.SeedAdminAsync(UserManeger, roleManeger);
         await ContextSeed.SeedModulesAsync(contextservice);
    }
    catch (Exception ex)
    {
        var logger = LoggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }

}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.UseRouting();
app.UseAuthentication(); ;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

app.Run();
