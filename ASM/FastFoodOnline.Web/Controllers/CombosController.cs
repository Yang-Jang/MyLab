using FastFoodOnline.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOnline.Web.Controllers
{
    public class CombosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CombosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 6;
            var query = _context.Combos.Where(c => c.IsActive).OrderByDescending(c => c.CreatedAt);
            
            var totalItems = await query.CountAsync();
            var combos = await query.Skip((page-1)*pageSize).Take(pageSize).ToListAsync();
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            return View(combos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var combo = await _context.Combos
                .Include(c => c.ComboItems)
                .ThenInclude(ci => ci.Food)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (combo == null) return NotFound();

            return View(combo);
        }
    }
}
