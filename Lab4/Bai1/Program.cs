using Bai1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình kết nối Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Cấu hình Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 3. Cấu hình đường dẫn trang Login/AccessDenied
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 4. Cấu hình Phân quyền (Policy)
builder.Services.AddAuthorization(options =>
{
    // Policy Admin: Chỉ Admin mới được vào
    options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("Admin"));

    // Policy Sales: Dành cho chức năng Bán hàng (Create)
    // Logic: Người có Claim "Sales" HOẶC Claim "Admin" đều được chấp nhận
    options.AddPolicy("SalesPolicy", policy => 
        policy.RequireAssertion(context => 
            context.User.HasClaim(c => c.Type == "Sales") || 
            context.User.HasClaim(c => c.Type == "Admin")
        ));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// QUAN TRỌNG: Dòng này giúp nạp file CSS/JS trong thư mục wwwroot
app.UseStaticFiles();

app.UseRouting();

// Thứ tự bắt buộc: Authentication (Xác thực) -> Authorization (Phân quyền)
app.UseAuthentication();
app.UseAuthorization();

// Định tuyến
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();