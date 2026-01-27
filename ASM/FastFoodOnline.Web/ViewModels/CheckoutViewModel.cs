using System.ComponentModel.DataAnnotations;
using FastFoodOnline.Web.Models;

namespace FastFoodOnline.Web.ViewModels
{
    public class CheckoutViewModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        public string PaymentMethod { get; set; } = "COD"; // COD, MoMo, VNPay, ZaloPay
        
        public Cart? Cart { get; set; }
    }
}
