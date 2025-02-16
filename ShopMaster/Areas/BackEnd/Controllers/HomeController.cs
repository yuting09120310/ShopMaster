using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class HomeController : BaseController
    {
        private readonly shopmasterdbContext _db;

        public HomeController(shopmasterdbContext db) : base(db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            GetMenu();
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
