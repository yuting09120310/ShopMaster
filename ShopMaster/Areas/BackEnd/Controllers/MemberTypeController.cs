using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class MemberTypeController : BaseController
    {
        private readonly shopmasterdbContext _db;

        public MemberTypeController(shopmasterdbContext db) : base(db)
        {
            _db = db;
        }

        // 會員等級列表
        public async Task<IActionResult> Index()
        {
            var memberTypes = await _db.MemberTypes.OrderBy(m => m.UpgradeThreshold).ToListAsync();
            return View(memberTypes);
        }

        // 新增 - GET
        public IActionResult Create()
        {
            return View();
        }

        // 新增 - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberType memberType)
        {
            if (ModelState.IsValid)
            {
                memberType.CreatedAt = DateTime.UtcNow;
                _db.MemberTypes.Add(memberType);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(memberType);
        }

        // 編輯 - GET
        public async Task<IActionResult> Edit(int id)
        {
            var memberType = await _db.MemberTypes.FindAsync(id);
            if (memberType == null) return NotFound();
            return View(memberType);
        }

        // 編輯 - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MemberType memberType)
        {
            if (ModelState.IsValid)
            {
                _db.Update(memberType);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(memberType);
        }

        // 刪除
        public async Task<IActionResult> Delete(int id)
        {
            var memberType = await _db.MemberTypes.Include(m => m.Members).FirstOrDefaultAsync(m => m.Id == id);
            if (memberType == null) return NotFound();

            if (memberType.Members.Any()) // 如果該等級內有會員，禁止刪除
            {
                TempData["Error"] = "此等級仍有會員，無法刪除";
                return RedirectToAction("Index");
            }

            _db.MemberTypes.Remove(memberType);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
