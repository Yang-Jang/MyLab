using FastFoodOnline.Web.Models;

namespace FastFoodOnline.Web.ViewModels
{
    public class FoodListViewModel
    {
        public IEnumerable<Food> Foods { get; set; } = new List<Food>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        
        public string? CurrentCategorySlug { get; set; }
        public string? CurrentSearch { get; set; }
        public string? CurrentSort { get; set; } // price_asc, price_desc
        
        // Paging
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
