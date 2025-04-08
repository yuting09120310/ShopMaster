using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ShopMaster.Areas.FrontEnd.Models;
using ShopMaster.Areas.BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ShopMaster.Areas.FrontEnd.ViewModels;

namespace ShopMaster.Areas.FrontEnd.Controllers
{
    [Area("FrontEnd")]
    public class ProductsController : Controller
    {
        private readonly shopmasterdbContext _db;


        public ProductsController(shopmasterdbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Details(int id)
        {
            // 取得主商品資訊
            var product = _db.Products
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            var productimg = _db.ProductImages
                .Where(p => p.ProductId == id)
                .ToList();

            product.ProductImages = productimg;

            product.ProductSpecs = _db.ProductSpecs
                .Where(p => p.ProductId == id)
                .ToList();

            // 推薦同類型商品（排除自己）
            var recommended = _db.Products
                .Where(p => p.TypeId == product.TypeId && p.Id != product.Id)
                .OrderBy(r => Guid.NewGuid()) // 隨機排序（可改成熱門、評價等）
                .Take(4)
                .ToList();

            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                RecommendedProducts = recommended
            };

            return View(viewModel);
        }
        
    }
}

