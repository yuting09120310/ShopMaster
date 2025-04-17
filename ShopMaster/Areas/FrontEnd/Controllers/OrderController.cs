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
            if (string.IsNullOrEmpty(memberId))
            {
                return BadRequest("會員未登入");
            }

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



            CheckoutPageViewModel viewModel = new CheckoutPageViewModel();
            viewModel.Carts = carts;
            viewModel.Member = member;
            viewModel.Ecoupons = validEcoupons;
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


            CheckoutPageViewModel viewModel = new CheckoutPageViewModel();
            viewModel.Carts = carts;
            viewModel.Member = member;
            viewModel.Ecoupons = validEcoupons;
            viewModel.SelectedEcoupon = _db.Ecoupons.Where(x => x.Id == ecouponId).FirstOrDefault();
            viewModel.Payinfo = _db.PayInfos.Where(x => x.Publish == 1).ToList();

            return PartialView("_OrderDetails", viewModel);
        }


        [HttpPost]
        public ActionResult Checkout(CheckoutFormViewModel viewModel)
        {
            // 從Session取得會員編號
            string? memberId = HttpContext.Session.GetString("MemberId");
            if (string.IsNullOrEmpty(memberId))
            {
                return BadRequest("會員未登入");
            }

            // 抓db取得會員資訊
            Member member = _db.Members.FirstOrDefault(x => x.Id.ToString() == memberId);
            if (member == null)
            {
                return NotFound("找不到會員資料");
            }

            // 抓db取得會員等級折扣
            MemberType memberType = _db.MemberTypes.FirstOrDefault(x => x.Id == member.MemberTypeId);
            if (memberType == null)
            {
                return NotFound("找不到會員等級資料");
            }

            // 抓db取得會員購物車所有內容
            List<Cart> carts = _db.Carts.Where(x => x.MemberId.ToString() == memberId).ToList();

            // 訂單明細
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (Cart cart in carts)
            {
                // 取得購物車內商品編號之對應商品資訊
                Product p = _db.Products.FirstOrDefault(x => x.Id == cart.ProductId);
                if (p == null)
                {
                    return NotFound($"找不到商品編號 {cart.ProductId} 的商品資料");
                }

                OrderDetail o = new OrderDetail
                {
                    // 訂單明細 商品編號
                    ProductId = cart.ProductId,
                    // 訂單明細 商品原價
                    OriginalPrice = p.Price,
                    // 訂單明細 折扣完之商品最終金額
                    FinalPrice = p.Price * memberType.Discount,
                    // 訂單明細 該商品數量
                    Quantity = cart.Quantity,
                    // 訂單明細 該商品總價
                    SubTotal = (p.Price * memberType.Discount) * cart.Quantity
                };
                orderDetails.Add(o);
            }

            // 計算金額
            var totalAmount = orderDetails.Sum(x => x.SubTotal);

            // 訂單主檔
            Order order = new Order
            {
                MemberId = member.Id,
                MemberTypeId = member.MemberTypeId,
                TotalAmount = totalAmount,
                Status = 1,
                //PaymentType = viewModel.SelectedPayInfoId,
                PaymentType = 1,
                CreatedAt = DateTime.Now // 假設有 CreatedAt 欄位
            };

            // 儲存訂單主檔
            _db.Orders.Add(order);
            _db.SaveChanges(); // 此時 order.Id 已經被資料庫生成

            // 將訂單主檔的 Id 設置到每個訂單明細中
            foreach (var detail in orderDetails)
            {
                detail.OrderId = order.Id;
            }

            // 儲存訂單明細
            _db.OrderDetails.AddRange(orderDetails);
            // 刪除購物車內所有商品
            _db.Carts.RemoveRange(carts); 
            _db.SaveChanges();


            // 重導到成功頁面
            return RedirectToAction("OrderSuccess", "Order", new { area = "FrontEnd", orderId = order.Id });
        }


        public IActionResult OrderSuccess(long orderId)
        {
            // 從資料庫取得訂單資訊
            var order = _db.Orders
                .Include(o => o.PaymentTypeNavigation)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("找不到訂單資訊");
            }

            // 建立 ViewModel
            var viewModel = new OrderSuccessViewModel
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt ?? DateTime.Now,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentTypeNavigation?.Name ?? "未知"
            };

            return View(viewModel);
        }

    }
}
