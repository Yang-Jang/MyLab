using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bai4.Data;
using Bai4.Models;


var builder = WebApplication.CreateBuilder(args);

// Dbcontext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity và password policy
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {

    options.Password.RequireDigit = true;  
    options.Password.RequiredLength = 8;   
    options.Password.RequireNonAlphanumeric = true;  
    options.Password.RequireUppercase = true;  
    options.Password.RequireLowercase = true;  
    
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;  
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add services to the container.
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
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
