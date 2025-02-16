using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class ProductController : BaseController
    {
        private readonly shopmasterdbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProductController(shopmasterdbContext db, IWebHostEnvironment env) : base(db)
        {
            _db = db;
            _env = env;
        }

        // 商品列表
        public async Task<IActionResult> Index()
        {
            var products = await _db.Products.Include(p => p.Type).OrderByDescending(p => p.CreatedAt).ToListAsync();
            return View(products);
        }

        // 新增 - GET
        public async Task<IActionResult> Create()
        {

            ViewBag.ProductTypes = await _db.ProductTypes.ToListAsync();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? mainImageFile, List<IFormFile>? imageFiles)
        {
            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.UtcNow;
                product.Publish = product.Publish ?? 0; // 預設為未啟用
                _db.Products.Add(product);
                await _db.SaveChangesAsync(); // 先儲存，取得商品 ID

                //  1. 設定商品圖片儲存路徑 (upload/product/{商品編號}/)
                string productFolder = Path.Combine(_env.WebRootPath, "upload", "product", product.Id.ToString());
                if (!Directory.Exists(productFolder))
                {
                    Directory.CreateDirectory(productFolder);
                }

                //  2. 儲存主要圖片
                if (mainImageFile != null)
                {
                    string mainFileName = $"main{Path.GetExtension(mainImageFile.FileName)}"; // 檔名: main.jpg
                    string mainFilePath = Path.Combine(productFolder, mainFileName);

                    using (var stream = new FileStream(mainFilePath, FileMode.Create))
                    {
                        await mainImageFile.CopyToAsync(stream);
                    }

                    product.MainImage = $"/upload/product/{product.Id}/{mainFileName}";
                    _db.Products.Update(product);
                    await _db.SaveChangesAsync();
                }

                //  3. 儲存多張商品圖片
                if (imageFiles != null)
                {
                    foreach (var image in imageFiles)
                    {
                        string imgFileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        string imgFilePath = Path.Combine(productFolder, imgFileName);

                        using (var stream = new FileStream(imgFilePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        _db.ProductImages.Add(new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = $"/upload/product/{product.Id}/{imgFileName}",
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            ViewBag.ProductTypes = await _db.ProductTypes.ToListAsync();
            return View(product);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            ViewBag.ProductTypes = await _db.ProductTypes.ToListAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile? mainImageFile, List<IFormFile>? imageFiles, List<long>? deleteImageIds)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == product.Id);
                if (existingProduct == null) return NotFound();

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Content = product.Content;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.TypeId = product.TypeId;
                existingProduct.Publish = product.Publish;

                //  1. 設定商品圖片資料夾 (upload/product/{商品編號}/)
                string productFolder = Path.Combine(_env.WebRootPath, "upload", "product", existingProduct.Id.ToString());
                if (!Directory.Exists(productFolder))
                {
                    Directory.CreateDirectory(productFolder);
                }

                //  2. 更新主要圖片
                if (mainImageFile != null)
                {
                    // 刪除舊圖片
                    if (!string.IsNullOrEmpty(existingProduct.MainImage))
                    {
                        string oldImgPath = Path.Combine(_env.WebRootPath, existingProduct.MainImage.TrimStart('/'));
                        if (System.IO.File.Exists(oldImgPath)) System.IO.File.Delete(oldImgPath);
                    }

                    string mainFileName = $"main{Path.GetExtension(mainImageFile.FileName)}"; // 新圖片: main.jpg
                    string mainFilePath = Path.Combine(productFolder, mainFileName);

                    using (var stream = new FileStream(mainFilePath, FileMode.Create))
                    {
                        await mainImageFile.CopyToAsync(stream);
                    }

                    existingProduct.MainImage = $"/upload/product/{existingProduct.Id}/{mainFileName}";
                }

                //  3. 刪除選擇的多圖片
                if (deleteImageIds != null)
                {
                    var imagesToDelete = _db.ProductImages.Where(img => deleteImageIds.Contains(img.Id)).ToList();
                    foreach (var img in imagesToDelete)
                    {
                        string imgPath = Path.Combine(_env.WebRootPath, img.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imgPath)) System.IO.File.Delete(imgPath);

                        _db.ProductImages.Remove(img); // 從資料庫刪除
                    }
                }

                //  4. 新增多圖片
                if (imageFiles != null)
                {
                    foreach (var image in imageFiles)
                    {
                        string imgFileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        string imgFilePath = Path.Combine(productFolder, imgFileName);

                        using (var stream = new FileStream(imgFilePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        _db.ProductImages.Add(new ProductImage
                        {
                            ProductId = existingProduct.Id,
                            ImageUrl = $"/upload/product/{existingProduct.Id}/{imgFileName}",
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                _db.Products.Update(existingProduct);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProductTypes = await _db.ProductTypes.ToListAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var product = await _db.Products
                .Include(p => p.ProductImages) // 取得關聯的圖片
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            // 1. 刪除所有商品圖片檔案
            string productFolder = Path.Combine(_env.WebRootPath, "upload", "product", product.Id.ToString());

            if (Directory.Exists(productFolder))
            {
                Directory.Delete(productFolder, true); // 刪除資料夾 & 內部所有圖片
            }

            //  2. 刪除資料庫中的圖片紀錄
            _db.ProductImages.RemoveRange(product.ProductImages);

            //  3. 刪除商品
            _db.Products.Remove(product);

            //  4. 儲存變更
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }


    }
}
