using IdentityMail.Web.Context;
using IdentityMail.Web.CustomValidation;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Sen SQL server kullanacaksýn baðlantý adresin de þu diye belirttik aþaðýda
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    
}).AddEntityFrameworkStores<AppDbContext>()
.AddErrorDescriber<CustomErrorDescriber>()
.AddDefaultTokenProviders(); //Girilen mail adresi için kullanýcýnýn mailine link göderiyoruz.Token tabanlý çalýþýyor. Bu metot olmazsa biz o tokený linki üretemiyoruz. 

builder.Services.AddAuthentication();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.Cookie.Name = "IdentityMailCookie";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

using(var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    if(!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new AppRole { Name = "Admin" });
    }

    if(!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new AppRole { Name = "User" });
    }
}

app.Run();
