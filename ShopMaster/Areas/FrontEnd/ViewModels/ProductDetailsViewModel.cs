using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.FrontEnd.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }

        public List<Product> RecommendedProducts { get; set; }
    }
}
