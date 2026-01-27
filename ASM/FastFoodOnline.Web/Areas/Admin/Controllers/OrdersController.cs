using FastFoodOnline.Web.Data;
using FastFoodOnline.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOnline.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminRole")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Orders
        // Xem danh sách tất cả đơn hàng
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
    .Include(o => o.OrderItems)
    .OrderByDescending(o => o.CreatedAt)
    .ToListAsync();

            return View(orders);
        }

        // GET: Admin/Orders/Details/5
        // Xem chi tiết đơn hàng
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Food)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Combo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Orders/UpdateStatus
        // Cập nhật trạng thái thanh toán hoặc giao hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
             var order = await _context.Orders.FindAsync(id);
             if (order == null) return NotFound();

             // Demo logic cập nhật trạng thái đơn giản
             // Có thể mở rộng thành enum
             if(status == "Paid")
             {
                 order.PaymentStatus = "Paid";
             }
             else if (status == "Pending")
             {
                 order.PaymentStatus = "Pending";
             }
             // order.OrderStatus = ... (Nếu có field này)

             _context.Update(order);
             await _context.SaveChangesAsync();

             return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}
