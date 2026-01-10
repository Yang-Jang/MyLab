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

// 2. CẤU HÌNH IDENTITY
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 3. CẤU HÌNH EMAIL SERVICE
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// --- ĐOẠN QUAN TRỌNG ĐỂ FIX LỖI ---
// Chỉ định rõ namespace đầy đủ để tránh xung đột
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();
// -----------------------------------

// 4. THÊM MVC & RAZOR PAGES
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// --- PIPELINE ---
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