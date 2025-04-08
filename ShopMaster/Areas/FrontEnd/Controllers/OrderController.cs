using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModels;

namespace ShopMaster.Areas.FrontEnd.Controllers
{
    [Area("FrontEnd")]
    public class OrderController : Controller
    {
        protected readonly shopmasterdbContext _db;

        public OrderController(shopmasterdbContext db)
        {
            _db = db;
        }

        public ActionResult Checkout()
        {
            string? memberId = HttpContext.Session.GetString("MemberId");
            List<Cart> carts = _db.Carts.Where(x => x.MemberId.ToString() == memberId).ToList();

            foreach(Cart cart in carts)
            {
                cart.Product = _db.Products.Where(x => x.Id == cart.ProductId).FirstOrDefault();
            }

            Member member = _db.Members.FirstOrDefault(x => x.Id.ToString() == memberId);
            member.MemberType = _db.MemberTypes.FirstOrDefault(x => x.Id == member.Id);

            CheckoutViewModel viewModel = new CheckoutViewModel();
            viewModel.Carts = carts;
            viewModel.Member = member;

            return View(viewModel);
        }
    }
}
