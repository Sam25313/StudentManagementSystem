
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequiredLength = 10;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageStudents", policy =>
        policy.RequireRole("Administrator", "Admissions"));

    options.AddPolicy("ManageGuardians", policy =>
        policy.RequireRole("Administrator", "Admissions"));

    options.AddPolicy("ViewStudentContacts", policy =>
        policy.RequireRole(
            "Administrator", "Admissions", "Accounts"));
});

builder.Services.AddScoped<StudentRegistrationService>();
builder.Services.AddScoped<StudentPhotoService>();

var app = builder.Build();

// Create roles and assign the existing administrator account.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        services.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles =
    {
        "Administrator",
        "Admissions",
        "Accounts"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(
                new IdentityRole(role));

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create role '{role}': " +
                    string.Join("; ",
                        result.Errors.Select(e => e.Description)));
            }
        }
    }

    // IMPORTANT: Use the email of your own registered account.
    var adminEmail = "samuelbarasa2019@gmail.com";

    var adminUser =
        await userManager.FindByEmailAsync(adminEmail);

    if (adminUser != null &&
        !await userManager.IsInRoleAsync(
            adminUser, "Administrator"))
    {
        var result = await userManager.AddToRoleAsync(
            adminUser, "Administrator");

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Failed to assign Administrator role: " +
                string.Join("; ",
                    result.Errors.Select(e => e.Description)));
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
