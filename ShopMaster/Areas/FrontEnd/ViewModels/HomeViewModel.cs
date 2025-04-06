using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.FrontEnd.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> HotProducts { get; set; }

        public List<Product> NewsProducts { get; set; }
    }
}
