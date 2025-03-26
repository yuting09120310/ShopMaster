using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.FrontEnd.Controllers
{
    [Area("FrontEnd")]
    public class MemberController : Controller
    {
        protected readonly shopmasterdbContext _db;

        public MemberController(shopmasterdbContext db)
        {
            _db = db;
        }
        
        public IActionResult Index()
        {
            string? memberId = HttpContext.Session.GetString("MemberId");

            if (memberId == null)
            {
                return RedirectToAction("Login", "Member", new { Area = "FrontEnd" });
            }

            var member = _db.Members.FirstOrDefault(x => x.Id.ToString() == memberId);


            return View(member);
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(string Account, string Password)
        {
            if (string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "❌ 帳號與密碼不得為空！";
                return View();
            }

            // 加密密碼（假設密碼是 Hash 儲存）
            string hashedPassword = HashPassword(Password);

            // 查詢 MySQL 是否有這個帳號
            var member = _db.Members.FirstOrDefault(a => a.Email == Account && a.PasswordHash == hashedPassword);

            if (member == null)
            {
                ViewBag.Error = "❌ 帳號或密碼錯誤，請再試一次！";
                return View();
            }

            // 設定登入 Session
            HttpContext.Session.SetString("MemberId", member.Id.ToString());
            return RedirectToAction("Index", "Home", new { Area = "FrontEnd" });
        }


        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("MemberId");
            return RedirectToAction("Index", "Home", new { Area = "FrontEnd" });
        }
    }
}
