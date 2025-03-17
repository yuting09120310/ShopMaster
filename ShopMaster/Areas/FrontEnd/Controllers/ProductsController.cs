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
    public class ProductsController : Controller
    {
        private readonly shopmasterdbContext _db;


        public ProductsController(shopmasterdbContext db)
        {
            _db = db;
        }

        // GET: ProductsController
        // 取產品資料
        //public async Task<IActionResult>  Index()
        //{
        //    var productList = await _db.Products.ToListAsync();


        //    var productF = productList.Select(p => new ViewModelsF.Product
        //    {
        //        Name = p.Name,
        //        Id = p.Id,
        //        Description = p.Description,
        //        Price = p.Price

        //    }).ToList(); 


        //    return View(productF);
        //}


        public async Task<IActionResult> index()
        {
            var product = await _db.Products.ToListAsync();
            var productTyoe = await _db.ProductTypes.ToListAsync();

            var result = product.Join(productTyoe,
                                        p => p.TypeId,
                                        pt => pt.Id,
                                        (p, pt) => new ViewModelsF.Product
                                        {
                                            TypeId = pt.Id,
                                            Name = p.Name,




                                        }).ToList();




            return View(result);
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

