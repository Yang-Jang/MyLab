using FastFoodOnline.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FastFoodOnline.Web.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // 1. Seed Roles (Tạo các vai trò)
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!await roleManager.RoleExistsAsync("Merchant")) // Thêm role Merchant (Người bán hàng)
                    await roleManager.CreateAsync(new IdentityRole("Merchant"));
                if (!await roleManager.RoleExistsAsync("Customer"))
                    await roleManager.CreateAsync(new IdentityRole("Customer"));

                // 2. Seed Admin User (Tạo tài khoản Admin mặc định)
                var adminEmail = "admin@local.test";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new IdentityUser { 
                        UserName = adminEmail, 
                        Email = adminEmail, 
                        EmailConfirmed = true 
                    };
                    var result = await userManager.CreateAsync(adminUser, "Password@123");
                    if (result.Succeeded)
                    {
                        // Gán Role Admin
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        
                        // Seed Claims (Gán Quyền hạn cụ thể)
                        // Gán Claim "Permission" với giá trị "ManageStore" cho user Admin
                        // Điều này cho phép user này vượt qua Policy "RequireAdminRole" đã định nghĩa trong Program.cs
                        await userManager.AddClaimAsync(adminUser, new Claim("Permission", "ManageStore"));
                    }
                }
                
                // Seed Merchant User (Tạo tài khoản Merchant mặc định để test)
                var merchantEmail = "merchant@local.test";
                if (await userManager.FindByEmailAsync(merchantEmail) == null)
                {
                    var merchantUser = new IdentityUser {
                        UserName = merchantEmail,
                        Email = merchantEmail,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(merchantUser, "Password@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(merchantUser, "Merchant");
                        // Merchant cũng có quyền quản lý cửa hàng
                        await userManager.AddClaimAsync(merchantUser, new Claim("Permission", "ManageStore"));
                    }
                }

                // 3. Seed Categories (Tạo danh mục món ăn)
                if (!context.Categories.Any())
                {
                    var categories = new List<Category>
                    {
                        new Category { Name = "Burgers", Slug = "burgers" },
                        new Category { Name = "Pizza", Slug = "pizza" },
                        new Category { Name = "Đồ uống", Slug = "drinks" }, // Việt hóa tên
                        new Category { Name = "Món ăn kèm", Slug = "sides" },
                        new Category { Name = "Gà rán", Slug = "chicken" }
                    };
                    context.Categories.AddRange(categories);
                    await context.SaveChangesAsync();
                }

                // 4. Seed Foods (Tạo món ăn mẫu)
                if (!context.Foods.Any())
                {
                    // Lấy ID của các category vừa tạo
                    var catBurgers = context.Categories.First(c => c.Slug == "burgers");
                    var catPizza = context.Categories.First(c => c.Slug == "pizza");
                    var catDrinks = context.Categories.First(c => c.Slug == "drinks");
                    
                    var foods = new List<Food>();
                    
                    // Burgers
                    for (int i = 1; i <= 5; i++)
                    {
                        foods.Add(new Food { 
                            Name = $"Burger Cổ Điển {i}", 
                            Price = 50000 + i*5000, 
                            CategoryId = catBurgers.Id, 
                            Description = "Bò nướng lửa hồng, rau tươi ngon.", 
                            ImageUrl = "/images/placeholder_burger.jpg" 
                        });
                    }
                     // Pizza
                    for (int i = 1; i <= 5; i++)
                    {
                        foods.Add(new Food { 
                            Name = $"Pizza Pepperoni {i}", 
                            Price = 150000 + i*10000, 
                            CategoryId = catPizza.Id, 
                            Description = "Pizza phô mai béo ngậy.", 
                            ImageUrl = "/images/placeholder_pizza.jpg" 
                        });
                    }
                    // Drinks
                    for (int i = 1; i <= 5; i++)
                    {
                        foods.Add(new Food { 
                            Name = $"Coca Cola {i}", 
                            Price = 15000, 
                            CategoryId = catDrinks.Id, 
                            Description = "Nước ngọt có ga giải khát.", 
                            ImageUrl = "/images/placeholder_drink.jpg" 
                        });
                    }
                    
                    context.Foods.AddRange(foods);
                    await context.SaveChangesAsync();
                }
                
                // 5. Seed Combos (Tạo Combo mẫu)
                if (!context.Combos.Any())
                {
                     var combos = new List<Combo>
                     {
                         new Combo { 
                             Name = "Combo Gia Đình", 
                             Price = 250000, 
                             Description = "1 Pizza lớn + 2 Cocacola", 
                             ImageUrl = "/images/combo1.jpg" 
                         },
                         new Combo { 
                             Name = "Combo Cặp Đôi", 
                             Price = 120000, 
                             Description = "2 Burger bò + 2 Cocacola", 
                             ImageUrl = "/images/combo2.jpg" 
                         }
                     };
                     context.Combos.AddRange(combos);
                     await context.SaveChangesAsync();
                }
            }
        }
    }
}
