using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.BackEnd.ViewModels;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class OrderController : BaseController
    {
        private readonly shopmasterdbContext _db;

        public OrderController(shopmasterdbContext db): base(db)
        {
            _db = db;
        }

        // 訂單列表
        public async Task<IActionResult> Index()
        {
            var orders = await _db.Orders
                .Include(o => o.Member)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // 訂單詳情
        public async Task<IActionResult> Details(long id)
        {
            var order = await _db.Orders
                .Include(o => o.Member)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.Ecoupons) //  載入優惠券
                .ThenInclude(e => e.EcouponEvent)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            //  計算會員折扣
            var discount = _db.MemberTypes.Where(x => x.Id == order.MemberTypeId).FirstOrDefault().Discount;
            decimal totalAmount = order.OrderDetails.Sum(od => od.Quantity * od.OriginalPrice);
            decimal customPay = order.OrderDetails.Sum(od => od.Quantity * od.OriginalPrice * (discount / 1));

            decimal memberDiscountTotal = totalAmount - customPay;


            //  計算優惠券折扣
            decimal couponDiscountTotal = order.Ecoupons.Sum(ec => ec.EcouponEvent.Discount);

            //  計算最終金額
            decimal finalAmount = order.TotalAmount - (memberDiscountTotal + couponDiscountTotal);

            //  轉換為 ViewModel
            var viewModel = new OrderDetailViewModel
            {
                OrderId = order.Id,
                MemberName = order.Member.Name,
                MemberEmail = order.Member.Email,
                TotalAmount = order.TotalAmount,
                FinalAmount = finalAmount, //  計算最終付款金額
                MemberTotalDiscount = memberDiscountTotal, //  會員折扣
                CouponTotalDiscount = couponDiscountTotal, //  優惠券折扣
                PaymentType = order.PaymentType == 1 ? "信用卡" : "銀行轉帳",
                OrderStatus = order.Status switch
                {
                    1 => "已完成",
                    2 => "待出貨",
                    _ => "取消"
                },
                CreatedAt = order.CreatedAt,
                OrderItems = order.OrderDetails.Select(od => new OrderItemViewModel
                {
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity,
                    OriginalPrice = od.OriginalPrice,
                    SubTotal = od.SubTotal
                }).ToList(),
                UsedCoupons = order.Ecoupons.Select(ec => new EcouponViewModel
                {
                    Code = ec.Code, //  優惠券代碼
                    Discount = ec.EcouponEvent.Discount //  優惠券折扣金額
                }).ToList()
            };

            return View(viewModel);
        }

        // 編輯 - GET
        public async Task<IActionResult> Edit(long id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // 編輯 - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                _db.Update(order);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // 刪除
        public async Task<IActionResult> Delete(long id)
        {
            var order = await _db.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            if (order.OrderDetails.Any()) // 如果該訂單有商品，禁止刪除
            {
                TempData["Error"] = "此訂單內有商品，無法刪除";
                return RedirectToAction("Index");
            }

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
