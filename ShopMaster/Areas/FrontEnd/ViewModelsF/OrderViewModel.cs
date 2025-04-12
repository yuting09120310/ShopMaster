using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class OrderViewModel
    {
        //購物車
        public List<Cart> cartList { get; set; } = new List<Cart>();

        public PayInfo payInfo { get; set; } = null!;

    }
}
