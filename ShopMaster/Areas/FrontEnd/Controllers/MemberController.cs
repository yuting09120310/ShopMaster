using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModels;



namespace ShopMaster.Areas.FrontEnd.Controllers
{
    [Area("FrontEnd")]
    public class MemberController : Controller
    {
        protected readonly shopmasterdbContext _db;
        private readonly IWebHostEnvironment _env;

        public MemberController(shopmasterdbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            string? memberId = HttpContext.Session.GetString("MemberId");

            if (memberId == null)
            {
                return RedirectToAction("Login", "Member", new { Area = "FrontEnd" });
            }

            var member = _db.Members.FirstOrDefault(x => x.Id.ToString() == memberId);

            return View();
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            // 假設你有一個方法來獲取當前用戶的 ID
            string? memberId = HttpContext.Session.GetString("MemberId");
            var member = _db.Members.Find(Convert.ToInt64(memberId));

            if (member == null)
            {
                return NotFound();
            }

            var viewModel = new EditProfileViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Address = member.Address,
                Birthday = member.Birthday,
                Avatar = member.Avatar
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditProfile(EditProfileViewModel model, IFormFile? AvatarFile)
        {
            if (ModelState.IsValid)
            {
                //  儲存頭像
                if (AvatarFile != null)
                {
                    string memberFolder = Path.Combine(_env.WebRootPath, "upload", "member", model.Id.ToString());
                    if (!Directory.Exists(memberFolder))
                    {
                        Directory.CreateDirectory(memberFolder);
                    }

                    string fileName = $"avatar{Path.GetExtension(AvatarFile.FileName)}";
                    string filePath = Path.Combine(memberFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        AvatarFile.CopyTo(stream);
                    }

                    model.Avatar = $"/upload/member/{model.Id}/{fileName}";
                }



                var existingMember = _db.Members.Find(model.Id);
                if (existingMember == null)
                {
                    return NotFound();
                }

                existingMember.Name = model.Name;
                existingMember.Email = model.Email;
                existingMember.Phone = model.Phone;
                existingMember.Address = model.Address;
                existingMember.Birthday = model.Birthday;
                existingMember.Avatar = model.Avatar;

                _db.SaveChanges();

                return RedirectToAction("Index", "Member", new { area = "FrontEnd" });
            }

            return View();
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
