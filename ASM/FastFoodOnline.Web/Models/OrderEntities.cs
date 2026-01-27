using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FastFoodOnline.Web.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<CartItem>? Items { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart? Cart { get; set; }
        
        public int? FoodId { get; set; }
        public Food? Food { get; set; }
        
        public int? ComboId { get; set; }
        public Combo? Combo { get; set; }
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string Code { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = "COD"; // COD, MoMo, VNPay...
        public string PaymentStatus { get; set; } = "Unpaid"; // Paid, Unpaid
        public string OrderStatus { get; set; } = "Pending"; // Pending, Preparing, Delivering, Delivered, Cancelled
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        public ICollection<OrderItem>? OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        
        public int? FoodId { get; set; }
        public Food? Food { get; set; }
        
        public int? ComboId { get; set; }
        public Combo? Combo { get; set; }
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }
}
