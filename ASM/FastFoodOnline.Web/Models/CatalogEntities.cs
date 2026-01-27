using System.ComponentModel.DataAnnotations;

namespace FastFoodOnline.Web.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(100)]
        public string Slug { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        
        public ICollection<Food>? Foods { get; set; }
    }

    public class Food
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class Combo
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public ICollection<ComboItem>? ComboItems { get; set; }
    }

    public class ComboItem
    {
        public int ComboId { get; set; }
        public Combo? Combo { get; set; }
        public int FoodId { get; set; }
        public Food? Food { get; set; }
        public int Quantity { get; set; }
    }
}
