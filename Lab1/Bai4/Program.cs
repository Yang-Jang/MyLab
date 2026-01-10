using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bai4.Data;
using Bai4.Services;
// KHÔNG using Microsoft.AspNetCore.Identity.UI.Services ở đây để tránh nhầm lẫn

var builder = WebApplication.CreateBuilder(args);

// 1. KẾT NỐI DATABASE
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); 

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

app.Run();