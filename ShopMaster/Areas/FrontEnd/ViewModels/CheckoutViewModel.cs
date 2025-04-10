using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.FrontEnd.ViewModels
{
    public class CheckoutViewModel
    {
        public List<Cart> Carts { get; set; }

        public Member Member { get; set; }

        public List<Ecoupon> Ecoupons { get; set; }

        public Ecoupon SelectEcoupon { get; set; }
    }
}
