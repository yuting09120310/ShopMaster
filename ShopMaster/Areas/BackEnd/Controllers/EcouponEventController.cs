using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.BackEnd.ViewModels;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class EcouponEventController : BaseController
    {
        private readonly shopmasterdbContext _db;

        public EcouponEventController(shopmasterdbContext db) : base(db)
        {
            _db = db;
        }

        // 優惠活動列表
        public async Task<IActionResult> Index()
        {
            var events = await _db.EcouponEvents.OrderByDescending(e => e.CreatedAt).ToListAsync();
            return View(events);
        }

        // 新增 - GET
        public IActionResult Create()
        {
            return View();
        }

        // 新增 - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EcouponEvent ecouponEvent)
        {
            if (ModelState.IsValid)
            {
                ecouponEvent.CreatedAt = DateTime.UtcNow;
                _db.EcouponEvents.Add(ecouponEvent);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ecouponEvent);
        }

        // 編輯 - GET
        public async Task<IActionResult> Edit(long id)
        {
            var ecouponEvent = await _db.EcouponEvents.FindAsync(id);
            if (ecouponEvent == null) return NotFound();
            return View(ecouponEvent);
        }

        // 編輯 - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EcouponEvent ecouponEvent)
        {
            if (ModelState.IsValid)
            {
                _db.Update(ecouponEvent);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ecouponEvent);
        }

        // 刪除
        public async Task<IActionResult> Delete(long id)
        {
            var ecouponEvent = await _db.EcouponEvents.Include(e => e.Ecoupons).FirstOrDefaultAsync(e => e.Id == id);
            if (ecouponEvent == null) return NotFound();

            if (ecouponEvent.Ecoupons.Any()) // 如果已有發出的優惠券，禁止刪除
            {
                TempData["Error"] = "該活動已有發出的優惠券，無法刪除";
                return RedirectToAction("Index");
            }

            _db.EcouponEvents.Remove(ecouponEvent);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // 顯示發放優惠券頁面
        public async Task<IActionResult> Distribute(long id)
        {
            var ecouponEvent = await _db.EcouponEvents.FindAsync(id);
            if (ecouponEvent == null) return NotFound();

            var members = await _db.Members.ToListAsync(); // 取得所有會員

            var viewModel = new EcouponDistributeViewModel
            {
                EventId = ecouponEvent.Id,
                EventName = ecouponEvent.Name,
                Members = members
            };

            return View(viewModel);
        }

        // 發放優惠券 - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Distribute(long EcouponEventId, long MemberId)
        {
            var ecouponEvent = await _db.EcouponEvents.FindAsync(EcouponEventId);
            if (ecouponEvent == null) return NotFound();

            var member = await _db.Members.FindAsync(MemberId);
            if (member == null) return NotFound();

            // 產生優惠券代碼
            string couponCode = $"{ecouponEvent.CodePrefix}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            var ecoupon = new Ecoupon
            {
                Code = couponCode,
                EcouponEventId = EcouponEventId,
                MemberId = MemberId,
                ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(ecouponEvent.ExpiryDays)),
                Status = 0, // 0 = 未使用
                CreatedAt = DateTime.UtcNow
            };

            _db.Ecoupons.Add(ecoupon);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"優惠券 {couponCode} 已成功發送給 {member.Name}！";
            return RedirectToAction("Index");
        }


    }
}
