using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class OrderViewModel
    {
        //購物車
        public List<Cart> cartList { get; set; } = new List<Cart>();

        // 總數量
        public Dictionary<long, long> total = new Dictionary<long, long>();

        //單品金額合計
        public Dictionary<long, decimal> price = new Dictionary<long, decimal>();

        //下拉選單
        public PayInfo payInfo { get; set; } = null!;

    }
}
