using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.BackEnd.ViewModels;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class DashboardController : BaseController
    {
        private readonly shopmasterdbContext _db;

        public DashboardController(shopmasterdbContext db) : base(db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            GetMenu();
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var monthOrders = _db.Orders
                .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Year == currentYear && x.CreatedAt.Value.Month == 2)
                .ToList();
            var monthOrderCount = monthOrders.Count;
            var monthTotalAmount = monthOrders.Sum(x => x.TotalAmount);

            var yearOrders = _db.Orders
                .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Year == currentYear)
                .ToList();
            var yearOrderCount = yearOrders.Count;
            var yearTotalAmount = yearOrders.Sum(x => x.TotalAmount);

            var dashboard = new DashBoard
            {
                MonthOrderCount = monthOrderCount,
                MonthTotalAmount = monthTotalAmount,
                YearOrderCount = yearOrderCount,
                YearTotalAmount = yearTotalAmount
            };

            return View(dashboard);
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
