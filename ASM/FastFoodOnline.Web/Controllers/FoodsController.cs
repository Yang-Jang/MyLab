using FastFoodOnline.Web.Data;
using FastFoodOnline.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOnline.Web.Controllers
{
    public class FoodsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FoodsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? categorySlug, string? searchString, string? sortOrder, int page = 1)
        {
            const int pageSize = 8;
            var query = _context.Foods.Include(f => f.Category).Where(f => f.IsActive);

            // Filter
            if (!string.IsNullOrEmpty(categorySlug))
            {
                query = query.Where(f => f.Category.Slug == categorySlug);
            }
            
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(f => f.Name.Contains(searchString) || f.Description.Contains(searchString));
            }

            // Sort
            query = sortOrder switch
            {
                "price_asc" => query.OrderBy(f => f.Price),
                "price_desc" => query.OrderByDescending(f => f.Price),
                _ => query.OrderByDescending(f => f.CreatedAt)
            };

            // Paging
            var totalItems = await query.CountAsync();
            var foods = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            
            var categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();

            var viewModel = new FoodListViewModel
            {
                Foods = foods,
                Categories = categories,
                CurrentCategorySlug = categorySlug,
                CurrentSearch = searchString,
                CurrentSort = sortOrder,
                PageIndex = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var food = await _context.Foods
                .Include(f => f.Category)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }
    }
}
