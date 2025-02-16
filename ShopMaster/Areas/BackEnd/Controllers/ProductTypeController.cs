using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class ProductTypeController : BaseController
        {
        private readonly shopmasterdbContext _db;

        public ProductTypeController(shopmasterdbContext db): base(db)
        {
            _db = db;
        }

        // 商品類別列表
        public async Task<IActionResult> Index()
        {
            var productTypes = await _db.ProductTypes.OrderBy(p => p.CreatedAt).ToListAsync();
            return View(productTypes);
        }

        // 新增 - GET
        public IActionResult Create()
        {
            return View();
        }

        // 新增 - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductType productType)
        {
            if (ModelState.IsValid)
            {
                productType.CreatedAt = DateTime.UtcNow;
                _db.ProductTypes.Add(productType);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(productType);
        }

        // 編輯 - GET
        public async Task<IActionResult> Edit(int id)
        {
            var productType = await _db.ProductTypes.FindAsync(id);
            if (productType == null) return NotFound();
            return View(productType);
        }

        // 編輯 - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductType productType)
        {
            if (ModelState.IsValid)
            {
                _db.Update(productType);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(productType);
        }

        // 刪除
        public async Task<IActionResult> Delete(int id)
        {
            var productType = await _db.ProductTypes.Include(p => p.Products).FirstOrDefaultAsync(p => p.Id == id);
            if (productType == null) return NotFound();

            if (productType.Products.Any()) // 如果類別下還有商品，禁止刪除
            {
                TempData["Error"] = "此類別下仍有商品，無法刪除";
                return RedirectToAction("Index");
            }

            _db.ProductTypes.Remove(productType);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
