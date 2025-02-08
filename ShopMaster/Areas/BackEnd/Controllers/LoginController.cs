using Microsoft.AspNetCore.Mvc;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string Username, string Password)
        {
            // 假設帳號是 admin，密碼是 1234
            if (Username == "admin" && Password == "1234")
            {
                // 設定登入 Session
                HttpContext.Session.SetString("AdminUser", Username);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "帳號或密碼錯誤！";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminUser");
            return RedirectToAction("Index");
        }
    }
}
