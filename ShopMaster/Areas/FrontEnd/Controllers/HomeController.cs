using System.ComponentModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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

        // 取產品資料
        public async Task<IActionResult> Index()
        {
            var product = await _db.Products.ToListAsync();
            var productType = await _db.ProductTypes.ToListAsync();            

            var result = product.Join(productType,
                                        p => p.TypeId,
                                        pt => pt.Id,
                                        (p, pt) => new ViewModelsF.Product
                                        {
                                            TypeId = pt.Id,
                                            Name = p.Name,
                                            Price = p.Price,
                                            MainImage = p.MainImage
                                            

                                        }).GroupBy(p => p.TypeId)
                                          .ToList();



            return View(result);
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

