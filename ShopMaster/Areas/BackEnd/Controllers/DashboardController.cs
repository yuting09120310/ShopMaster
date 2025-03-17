using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShopMaster.Areas.BackEnd.Models;

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
            var orders = _db.Orders
                .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Year == DateTime.Now.Year && x.CreatedAt.Value.Month == 2)
                .ToList();
            var orderCount = orders.Count;
            var totalAmt = orders.Sum(x => x.TotalAmount);

            return View();
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
