using FastFoodOnline.Web.Models;

namespace FastFoodOnline.Web.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Food> FeaturedFoods { get; set; } = new List<Food>();
        public IEnumerable<Combo> FeaturedCombos { get; set; } = new List<Combo>();
    }
}
