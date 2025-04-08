using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.FrontEnd.ViewModels
{
    public class CheckoutViewModel
    {
        public List<Cart> Carts { get; set; }

        public Member Member { get; set; }
    }
}
