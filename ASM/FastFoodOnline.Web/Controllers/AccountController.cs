using FastFoodOnline.Web.Extensions;
using FastFoodOnline.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastFoodOnline.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.Toast("Vui lòng kiểm tra lại thông tin đăng ký.", "warning");
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Role
                if (await _userManager.IsInRoleAsync(user, "Customer") == false)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                }

                // Claims extra info
                await _userManager.AddClaimAsync(user, new Claim("FullName", model.FullName ?? ""));
                await _userManager.AddClaimAsync(user, new Claim("Address", model.Address ?? ""));
                await _userManager.AddClaimAsync(user, new Claim("Gender", model.Gender ?? ""));
                await _userManager.AddClaimAsync(user, new Claim("DateOfBirth", model.DateOfBirth.ToString("yyyy-MM-dd")));

                await _signInManager.SignInAsync(user, isPersistent: false);

                this.Toast("Đăng ký thành công. Chào mừng bạn!", "success");
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            this.Toast("Đăng ký thất bại. Vui lòng thử lại.", "error");
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                this.Toast("Vui lòng nhập đúng Email và Mật khẩu.", "warning");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                this.Toast("Welcome back!", "success");
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            this.Toast("Đăng nhập thất bại. Sai tài khoản hoặc mật khẩu.", "error");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!string.IsNullOrWhiteSpace(remoteError))
            {
                this.Toast($"Google login lỗi: {remoteError}", "error");
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                this.Toast("Không lấy được thông tin đăng nhập từ Google.", "error");
                return RedirectToAction(nameof(Login));
            }

            // user đã có external login
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true
            );

            if (signInResult.Succeeded)
            {
                this.Toast("Đăng nhập Google thành công!", "success");
                return LocalRedirect(returnUrl);
            }

            // chưa có tài khoản -> tạo mới
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(email))
            {
                this.Toast("Google không trả về email. Không thể tạo tài khoản.", "error");
                return RedirectToAction(nameof(Login));
            }

            // Nếu email đã tồn tại (đã đăng ký thường) -> link external login vào user đó
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                if (!addLoginResult.Succeeded)
                {
                    this.Toast("Không thể liên kết Google với tài khoản hiện tại.", "error");
                    return RedirectToAction(nameof(Login));
                }

                await EnsureCustomerRole(existingUser);
                await EnsureFullNameClaim(existingUser, name);

                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                this.Toast("Đã liên kết Google và đăng nhập thành công!", "success");
                return LocalRedirect(returnUrl);
            }

            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                this.Toast("Tạo tài khoản từ Google thất bại.", "error");
                return RedirectToAction(nameof(Login));
            }

            var loginResult = await _userManager.AddLoginAsync(user, info);
            if (!loginResult.Succeeded)
            {
                this.Toast("Không thể thêm đăng nhập Google cho tài khoản.", "error");
                return RedirectToAction(nameof(Login));
            }

            await EnsureCustomerRole(user);
            await EnsureFullNameClaim(user, name);

            await _signInManager.SignInAsync(user, isPersistent: false);
            this.Toast("Google Login successful!", "success");
            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            this.Toast("Bạn đã đăng xuất.", "info");
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                this.Toast("Không tìm thấy người dùng.", "error");
                return RedirectToAction("Index", "Home");
            }

            var claims = await _userManager.GetClaimsAsync(user);

            var model = new ProfileViewModel
            {
                Email = user.Email,
                Phone = user.PhoneNumber,
                FullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value ?? "",
                Address = claims.FirstOrDefault(c => c.Type == "Address")?.Value ?? "",
                Gender = claims.FirstOrDefault(c => c.Type == "Gender")?.Value ?? "",
            };

            if (DateTime.TryParse(claims.FirstOrDefault(c => c.Type == "DateOfBirth")?.Value, out var dob))
                model.DateOfBirth = dob;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.Toast("Vui lòng kiểm tra lại thông tin.", "warning");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                this.Toast("Không tìm thấy người dùng.", "error");
                return RedirectToAction("Index", "Home");
            }

            user.PhoneNumber = model.Phone;
            await _userManager.UpdateAsync(user);

            // Replace claims
            var claims = await _userManager.GetClaimsAsync(user);
            var toRemove = claims.Where(c =>
                c.Type == "FullName" ||
                c.Type == "Address" ||
                c.Type == "Gender" ||
                c.Type == "DateOfBirth"
            ).ToList();

            if (toRemove.Any())
                await _userManager.RemoveClaimsAsync(user, toRemove);

            var newClaims = new List<Claim>
            {
                new Claim("FullName", model.FullName ?? ""),
                new Claim("Address", model.Address ?? ""),
                new Claim("Gender", model.Gender ?? ""),
                new Claim("DateOfBirth", model.DateOfBirth?.ToString("yyyy-MM-dd") ?? "")
            };

            await _userManager.AddClaimsAsync(user, newClaims);

            this.Toast("Cập nhật hồ sơ thành công!", "success");
            return RedirectToAction(nameof(Profile));
        }

        private async Task EnsureCustomerRole(IdentityUser user)
        {
            if (!await _userManager.IsInRoleAsync(user, "Customer"))
                await _userManager.AddToRoleAsync(user, "Customer");
        }

        private async Task EnsureFullNameClaim(IdentityUser user, string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return;

            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.Any(c => c.Type == "FullName")) return;

            await _userManager.AddClaimAsync(user, new Claim("FullName", fullName));
        }
    }
}
