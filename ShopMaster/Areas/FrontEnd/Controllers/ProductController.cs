using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ShopMaster.Areas.FrontEnd.Models;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.BackEnd.Controllers;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.FrontEnd.ViewModelsF;

namespace ShopMaster.Areas.FrontEnd.Controllers
{
    [Area("FrontEnd")]
    public class ProductController : BaseController
    {
        private readonly shopmasterdbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProductController(shopmasterdbContext db, IWebHostEnvironment env) : base(db)
        {
            _db = db;
            _env = env;
        }

        // GET: ProductsController
        public async Task<IActionResult>  Index()
        {
            var productList = await _db.Products.ToListAsync();


            var productF = productList.Select(p => new ViewModelsF.Product
            {
                Name = p.Name,
                Id = p.Id,
                Description = p.Description,
                Price = p.Price
            }).ToList(); 


            return View(productF);
        }

        // GET: ProductsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
