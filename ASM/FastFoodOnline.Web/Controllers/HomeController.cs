using FastFoodOnline.Web.Data;
using FastFoodOnline.Web.Models;
using FastFoodOnline.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FastFoodOnline.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync(),
                FeaturedFoods = await _context.Foods.Where(f => f.IsActive).OrderByDescending(f => f.CreatedAt).Take(4).ToListAsync(),
                FeaturedCombos = await _context.Combos.Where(c => c.IsActive).OrderByDescending(c => c.CreatedAt).Take(2).ToListAsync()
            };
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
