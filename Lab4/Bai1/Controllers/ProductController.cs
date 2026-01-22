using Bai1.Data;
using Bai1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Bai1.Controllers
{
    // Yêu cầu phải đăng nhập mới được vào Controller này
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. DANH SÁCH: Ai đăng nhập cũng xem được
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // 2. THÊM MỚI: Chỉ Sale hoặc Admin (Do Policy định nghĩa)
        [Authorize(Policy = "SalesPolicy")] 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Policy = "SalesPolicy")]
        [HttpPost]
        [ValidateAntiForgeryToken] // Thêm bảo mật chống giả mạo request
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                // Lưu ID người tạo (User hiện tại)
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                product.CreatedBy = userId;

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // 3. XEM CHI TIẾT
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // Kiểm tra quyền: Admin xem hết, Sale chỉ xem của mình
            if (!CheckAccess(product)) return RedirectToAction("AccessDenied", "Account");

            // Lấy tên người tạo để hiển thị
            if (!string.IsNullOrEmpty(product.CreatedBy))
            {
                var creator = await _userManager.FindByIdAsync(product.CreatedBy);
                ViewBag.CreatorName = creator != null ? creator.UserName : "Unknown User";
            }
            else
            {
                ViewBag.CreatorName = "System/Unknown";
            }

            return View(product);
        }

        // 4. SỬA
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (!CheckAccess(product)) return RedirectToAction("AccessDenied", "Account");

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();
            
            // Lấy sản phẩm gốc từ DB để kiểm tra quyền sở hữu
            var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            
            if (existingProduct == null) return NotFound();
            
            // Kiểm tra quyền trên sản phẩm gốc
            if (!CheckAccess(existingProduct)) return RedirectToAction("AccessDenied", "Account");

            if (ModelState.IsValid)
            {
                // Giữ nguyên người tạo ban đầu (không cho phép đổi người tạo)
                product.CreatedBy = existingProduct.CreatedBy; 
                
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // 5. XÓA
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (!CheckAccess(product)) return RedirectToAction("AccessDenied", "Account");

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                if (!CheckAccess(product)) return RedirectToAction("AccessDenied", "Account");

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Hàm kiểm tra quyền
        private bool CheckAccess(Product product)
        {
            // Admin luôn có quyền
            if (User.HasClaim(c => c.Type == "Admin")) return true;

            // Sale chỉ có quyền nếu là chính chủ
            if (User.HasClaim(c => c.Type == "Sales"))
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return product.CreatedBy == currentUserId;
            }

            return false;
        }
    }
}