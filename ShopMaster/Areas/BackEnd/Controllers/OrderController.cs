using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;

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
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
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
