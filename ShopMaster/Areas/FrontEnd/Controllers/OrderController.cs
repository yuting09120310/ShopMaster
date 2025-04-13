using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                cart.Product.ProductSpecs = _db.ProductSpecs.Where(x => x.ProductId == Convert.ToInt64(cart.ProductId)).ToList();
            }

            Member member = _db.Members.FirstOrDefault(x => x.Id.ToString() == memberId);
            member.MemberType = _db.MemberTypes.FirstOrDefault(x => x.Id == member.Id);

            var today = DateOnly.FromDateTime(DateTime.Now);
            decimal orderAmount = carts.Sum(item => item.Product.Price * item.Quantity);

            var validEcoupons = _db.Ecoupons
                .Include(x => x.EcouponEvent)
                .Where(x => x.MemberId.ToString() == memberId
                            && x.ExpiryDate >= today
                            && x.Status == 0
                            && x.EcouponEvent.StartDate <= today
                            && (x.EcouponEvent.EndDate == null || x.EcouponEvent.EndDate >= today)
                            && orderAmount >= x.EcouponEvent.MinOrderAmount)
                .ToList();



            CheckoutViewModel viewModel = new CheckoutViewModel();
            viewModel.Carts = carts;
            viewModel.Member = member;
            viewModel.Ecoupons = validEcoupons;
            viewModel.SelectEcoupon = new Ecoupon();
            viewModel.Payinfo = _db.PayInfos.Where(x => x.Publish == 1).ToList();

            return View(viewModel);
        }


        [HttpGet]
        public ActionResult UpdateOrderDetails(int? ecouponId)
        {
            string? memberId = HttpContext.Session.GetString("MemberId");
            List<Cart> carts = _db.Carts.Where(x => x.MemberId.ToString() == memberId).ToList();

            foreach (Cart cart in carts)
            {
                cart.Product = _db.Products.Where(x => x.Id == cart.ProductId).FirstOrDefault();
                cart.Product.ProductSpecs = _db.ProductSpecs.Where(x => x.ProductId == Convert.ToInt64(cart.ProductId)).ToList();
            }

            Member member = _db.Members.FirstOrDefault(x => x.Id.ToString() == memberId);
            member.MemberType = _db.MemberTypes.FirstOrDefault(x => x.Id == member.Id);

            var today = DateOnly.FromDateTime(DateTime.Now);
            decimal orderAmount = carts.Sum(item => item.Product.Price * item.Quantity);

            var validEcoupons = _db.Ecoupons
                .Include(x => x.EcouponEvent)
                .Where(x => x.MemberId.ToString() == memberId
                            && x.ExpiryDate >= today
                            && x.Status == 0
                            && x.EcouponEvent.StartDate <= today
                            && (x.EcouponEvent.EndDate == null || x.EcouponEvent.EndDate >= today)
                            && orderAmount >= x.EcouponEvent.MinOrderAmount)
                .ToList();


            CheckoutViewModel viewModel = new CheckoutViewModel();
            viewModel.Carts = carts;
            viewModel.Member = member;
            viewModel.Ecoupons = validEcoupons;
            viewModel.SelectEcoupon = _db.Ecoupons.Where(x => x.Id == ecouponId).FirstOrDefault();
            viewModel.Payinfo = _db.PayInfos.Where(x => x.Publish == 1).ToList();

            return PartialView("_OrderDetails", viewModel);
        }
    }
}
