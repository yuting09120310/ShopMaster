using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.FrontEnd.ViewModels
{
    public class CheckoutPageViewModel
    {
        // 從資料庫帶出的該會員購物車
        public List<Cart> Carts { get; set; }

        // 從資料庫帶出的客戶資訊
        public Member Member { get; set; }

        // 從資料庫帶出的優惠券
        public List<Ecoupon>? Ecoupons { get; set; }

        // 從資料庫帶出的付款資訊
        public List<PayInfo> Payinfo { get; set; }


        public Ecoupon? SelectedEcoupon { get; set; }
    }

    public class CheckoutFormViewModel
    {
        // 用戶從 Ecoupons 中選擇的優惠券 ID
        public long? SelectedEcouponId { get; set; }

        // 用戶從 Payinfo 中選擇的付款方式 ID
        public int SelectedPayInfoId { get; set; }

        // 用戶填寫的訂單備註
        public string? Notes { get; set; }
    }

    public class OrderSuccessViewModel
    {
        public long OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
    }

}
