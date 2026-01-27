using Microsoft.AspNetCore.Mvc;

namespace FastFoodOnline.Web.Extensions
{
    public static class ToastExtensions
    {
        public static void Toast(this Controller controller, string message, string type = "info", string? title = null)
        {
            controller.TempData["ToastMessage"] = message;
            controller.TempData["ToastType"] = type;   
            controller.TempData["ToastTitle"] = title; 
        }
    }
}
