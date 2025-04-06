using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Controllers;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModels;
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

        
        public IActionResult Index()
        {
            HomeViewModel viewModel = new HomeViewModel();

            // 抓取賣得最好的6個產品
            viewModel.HotProducts = _db.OrderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    Product = g.Key,
                    TotalQuantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(6)
                .Join(_db.Products, g => g.Product, p => p.Id, (g, p) => p)
                .ToList();

            // 抓取最新上架的6個產品
            viewModel.NewsProducts = _db.Products
                .OrderByDescending(p => p.Id) // 假設 Id 表示產品的上架時間
                .Take(6)
                .ToList();

            return View(viewModel);
        }
    }
}

