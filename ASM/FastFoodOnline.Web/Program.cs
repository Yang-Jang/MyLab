using FastFoodOnline.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ vào container.
// Lấy chuỗi kết nối từ file cấu hình (appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Không tìm thấy chuỗi kết nối 'DefaultConnection'.");

// Đăng ký ApplicationDbContext sử dụng SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Cấu hình Identity (Quản lý người dùng & Vai trò)
// Sử dụng IdentityUser mặc định và IdentityRole cho vai trò
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    // Cấu hình tùy chọn đăng nhập
    options.SignIn.RequireConfirmedAccount = false; // Không yêu cầu xác nhận email để đăng nhập (cho mục đích demo)
    
    // Cấu hình Policy mật khẩu (tùy chọn)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Lưu trữ dữ liệu Identity trong EF Core
.AddDefaultTokenProviders(); // Provider để sinh token reset pass, email confirmation...

// Cấu hình Authentication (Xác thực - Bạn là ai?)
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        // Cấu hình Google Login (nếu có)
        IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = googleAuthNSection["ClientId"] ?? "PLACEHOLDER_ID";
        options.ClientSecret = googleAuthNSection["ClientSecret"] ?? "PLACEHOLDER_SECRET";
    });

// Cấu hình Authorization (Ủy quyền - Bạn được làm gì?)
// Sử dụng Claims-based Authorization
builder.Services.AddAuthorization(options =>
{
    // Tạo một Policy tên là "RequireAdminRole"
    // Yêu cầu: User phải có Role là "Admin" HOẶC có Claim "Permission" với giá trị "ManageStore"
    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("Admin") || 
            context.User.HasClaim(c => c.Type == "Permission" && c.Value == "ManageStore")
        ));
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Cấu hình HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // Giá trị HSTS mặc định là 30 ngày. 
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Kích hoạt Middleware Xác thực (Authentication) trước
app.UseAuthentication();
// Kích hoạt Middleware Ủy quyền (Authorization) sau
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Seed Data (Dữ liệu mẫu)
// Tự động tạo Role Admin, User Admin và Claim khi ứng dụng chạy lần đầu
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedData(scope.ServiceProvider);
}

app.Run();
