using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bai4.Controllers
{
    [Authorize] 
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}