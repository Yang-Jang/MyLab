using FastFoodOnline.Web.Data;
using FastFoodOnline.Web.Extensions;
using FastFoodOnline.Web.Models;
using FastFoodOnline.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOnline.Web.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GetUserId() => _userManager.GetUserId(User) ?? "";

        private async Task<Cart?> GetCartAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Food)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Combo)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var cart = await GetCartAsync(userId);

            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                this.Toast("Giỏ hàng đang trống. Vui lòng thêm món trước khi thanh toán.", "warning");
                return RedirectToAction("Index", "Cart");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                this.Toast("Không tìm thấy người dùng.", "error");
                return RedirectToAction("Index", "Home");
            }

            var claims = await _userManager.GetClaimsAsync(user);

            var model = new CheckoutViewModel
            {
                FullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value ?? "",
                Address = claims.FirstOrDefault(c => c.Type == "Address")?.Value ?? "",
                Phone = user.PhoneNumber ?? "",
                Cart = cart
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Processing(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.Toast("Vui lòng kiểm tra lại thông tin thanh toán.", "warning");
                return RedirectToAction(nameof(Index));
            }

            var userId = GetUserId();
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                this.Toast("Giỏ hàng đang trống.", "warning");
                return RedirectToAction("Index", "Cart");
            }

            // Re-calculate total (security)
            decimal totalAmount = 0;
            foreach (var item in cart.Items)
            {
                if (item.Quantity < 1) item.Quantity = 1;
                totalAmount += item.Quantity * item.UnitPrice;
            }

            // Create Order
            var order = new Order
            {
                UserId = userId,
                FullName = model.FullName,
                Phone = model.Phone,
                Address = model.Address,
                TotalAmount = totalAmount,
                PaymentMethod = model.PaymentMethod,
                CreatedAt = DateTime.Now,
                OrderStatus = "Pending" // MVP
            };

            // Payment simulation
            order.PaymentStatus = (model.PaymentMethod == "COD") ? "Unpaid" : "Paid";

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create OrderItems
            var orderItems = cart.Items.Select(ci => new OrderItem
            {
                OrderId = order.Id,
                FoodId = ci.FoodId,
                ComboId = ci.ComboId,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice
            }).ToList();

            _context.OrderItems.AddRange(orderItems);

            // Clear Cart
            _context.CartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();

            this.Toast("Đặt hàng thành công! Quán sẽ xác nhận đơn sớm.", "success");
            return RedirectToAction(nameof(Success), new { orderId = order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Success(int orderId)
        {
            var userId = GetUserId();
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                this.Toast("Không tìm thấy đơn hàng.", "error");
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }
    }
}
