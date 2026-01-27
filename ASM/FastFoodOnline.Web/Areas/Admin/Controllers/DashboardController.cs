using FastFoodOnline.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOnline.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    // Áp dụng Policy "RequireAdminRole" thay vì chỉ kiểm tra Role
    [Authorize(Policy = "RequireAdminRole")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalOrders = await _context.Orders.CountAsync();
            // Tính tổng doanh thu từ các đơn hàng đã thanh toán
            ViewBag.TotalRevenue = await _context.Orders.Where(o => o.PaymentStatus == "Paid").SumAsync(o => o.TotalAmount);
            ViewBag.TotalFoods = await _context.Foods.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            
            return View();
        }
    }
}
