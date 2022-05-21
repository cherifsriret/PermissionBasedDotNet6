using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PermissionsBasedProject.Data;
using PermissionsBasedProject.Filters;
using PermissionsBasedProject.Seeds;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped < IAuthorizationHandler, PermissionAuthorizationHandler>();  
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString) );
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


//TODO: seed roles and default users
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var loggerfactory = services.GetRequiredService<ILoggerFactory>();
var logger = loggerfactory.CreateLogger("app");
logger.LogInformation("start Data seeded");
try
{


    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await DefaultRoles.SeedAsync(roleManager);
    await DefaultUsers.SeedBasicUserAsync(userManager);
    await DefaultUsers.SeedSuperAdminAsync(userManager, roleManager);

    logger.LogInformation("Data seeded");
    logger.LogInformation("Application Started");
}
catch (Exception)
{
    //logger.LogWarning(ex.Message, "An error occured while seeding the information");
}
app.Run();
