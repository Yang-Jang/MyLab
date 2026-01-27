using FastFoodOnline.Web.Data;
using FastFoodOnline.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOnline.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    // Áp dụng Policy "RequireAdminRole"
    [Authorize(Policy = "RequireAdminRole")]
    public class FoodsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public FoodsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/Foods
        // Hiển thị danh sách món ăn
        public async Task<IActionResult> Index()
        {
            var foods = _context.Foods.Include(f => f.Category);
            return View(await foods.ToListAsync());
        }

        // GET: Admin/Foods/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/Foods/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,ImageUrl,CategoryId")] Food food, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh nếu có
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "images", fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    food.ImageUrl = "/images/" + fileName;
                }
                else if (string.IsNullOrEmpty(food.ImageUrl)) 
                {
                     food.ImageUrl = "/images/placeholder.jpg"; // Ảnh mặc định
                }

                _context.Add(food);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", food.CategoryId);
            return View(food);
        }

        // GET: Admin/Foods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", food.CategoryId);
            return View(food);
        }

        // POST: Admin/Foods/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,ImageUrl,CategoryId")] Food food, IFormFile? imageFile)
        {
            if (id != food.Id)
            {
                return NotFound();
            }

            // ModelState có thể báo lỗi Category do validation, tạm bỏ qua check Category vì nó là Int
            // Tuy nhiên tốt nhất check kỹ. Ở đây giả sử valid nếu các trường chính ok.
            if (ModelState.IsValid)
            {
                try
                {
                     // Update logic...
                     // Nếu user upload ảnh mới
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(_environment.WebRootPath, "images", fileName);
                        
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        food.ImageUrl = "/images/" + fileName;
                    }
                    else 
                    {
                         // Nếu không upload ảnh mới, giữ nguyên URL cũ (cần query lại DB để lấy, hoặc Model Binding hidden field)
                         // Cách đơn giản: AsNoTracking query cái cũ để lấy ImageUrl nếu field này null, nhưng ở đây bind về
                         // Food food ở param: ImageUrl sẽ là null nếu form k gửi lên.
                         // Nên dùng AsNoTracking để lấy giá trị cũ.
                         var oldFood = await _context.Foods.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
                         if(oldFood != null && string.IsNullOrEmpty(food.ImageUrl))
                         {
                             food.ImageUrl = oldFood.ImageUrl; 
                         }
                    }

                    _context.Update(food);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodExists(food.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", food.CategoryId);
            return View(food);
        }

        // GET: Admin/Foods/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await _context.Foods
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }

        // POST: Admin/Foods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food != null)
            {
                _context.Foods.Remove(food);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FoodExists(int id)
        {
            return _context.Foods.Any(e => e.Id == id);
        }
    }
}
