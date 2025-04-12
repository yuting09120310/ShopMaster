using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class OrderViewModel
    {
        //購物車
        public List<Cart> cartList { get; set; } = new List<Cart>();

        //下拉選單
        public PayInfo payInfo { get; set; } = null!;

    }
}
