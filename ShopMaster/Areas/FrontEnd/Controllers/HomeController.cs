using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Controllers;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModelsF;
using ErrorViewModel = ShopMaster.Areas.FrontEnd.Models.ErrorViewModel;

namespace ShopMaster.Areas.FrontEnd.Controllers
{
    [Area("FrontEnd")]


    public class HomeController : Controller
    {
        private readonly shopmasterdbContext _db;


        private readonly ILogger<HomeController> _logger;

        public HomeController(shopmasterdbContext db, ILogger<HomeController> logger) 
        {
            _logger = logger;
            _db = db;
        }

        
        public async Task<IActionResult> Index(int? id)
        {  
            var product = await _db.Products.ToListAsync();
            var productType = await _db.ProductTypes.ToListAsync();            

            //取產品資料
            var productList = product.Join(productType,
                                        p => p.TypeId,
                                        pt => pt.Id,
                                        (p, pt) => new ViewModelsF.Products
                                        {
                                            TypeId = pt.Id,
                                            Id = p.Id,
                                            Name = p.Name,
                                            Price = p.Price,
                                            MainImage = p.MainImage,
                                            TypeName = pt.Name


                                        }).GroupBy(p => p.TypeId ?? 0)
                                          .ToList();

            // 點選下拉選單商品
            List<Products> productListLove = new List<Products>();
            if (id.HasValue)
            {
                 productListLove = product.Join(productType,
                                            p => p.TypeId,
                                            pt => pt.Id,
                                            (p, pt) => new ViewModelsF.Products
                                            {
                                                TypeId = pt.Id,
                                                Id = p.Id,
                                                Name = p.Name,
                                                Price = p.Price,
                                                MainImage = p.MainImage,
                                                TypeName = pt.Name


                                            }).Where(x => x.TypeId == id)
                                              .ToList();

            }


            var productsAll = new ProductsAll
            {
                ProductList = productList,
                ProductListLove = productListLove,
                ProductTypeList = productType
            };


            ViewData["TypeID"] = id;  

            return View(productsAll);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

