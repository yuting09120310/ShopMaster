using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class EcouponController : BaseController
    {
        private readonly shopmasterdbContext _db;

        public EcouponController(shopmasterdbContext db): base(db)
        {
            _db = db;
        }

        // 已發放的優惠券列表
        public async Task<IActionResult> Index(int? status, int? expired, string? eventName)
        {
            var query = _db.Ecoupons
                .Include(e => e.Member)
                .Include(e => e.EcouponEvent)
                .AsQueryable();

            // 篩選使用狀態
            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }

            // 篩選過期狀態
            if (expired.HasValue)
            {
                if (expired.Value == 1) // 已過期
                {
                    query = query.Where(e => e.ExpiryDate < DateOnly.FromDateTime(DateTime.UtcNow));
                }
                else // 未過期
                {
                    query = query.Where(e => e.ExpiryDate >= DateOnly.FromDateTime(DateTime.UtcNow));
                }
            }

            // 篩選活動名稱
            if (!string.IsNullOrEmpty(eventName))
            {
                query = query.Where(e => e.EcouponEvent.Name.Contains(eventName));
            }

            var coupons = await query.OrderByDescending(e => e.CreatedAt).ToListAsync();

            ViewBag.Status = status;
            ViewBag.Expired = expired;
            ViewBag.EventName = eventName;

            return View(coupons);
        }



        // 刪除未使用的優惠券
        public async Task<IActionResult> Delete(long id)
        {
            var coupon = await _db.Ecoupons.FindAsync(id);
            if (coupon == null) return NotFound();

            if (coupon.Status == 1) // 已使用的優惠券不能刪除
            {
                TempData["Error"] = "已使用的優惠券無法刪除";
                return RedirectToAction("Index");
            }

            _db.Ecoupons.Remove(coupon);
            await _db.SaveChangesAsync();
            TempData["Success"] = "優惠券刪除成功";
            return RedirectToAction("Index");
        }
    }
}
