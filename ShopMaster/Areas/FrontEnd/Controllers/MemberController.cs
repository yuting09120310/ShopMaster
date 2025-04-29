using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModelsF;
using ShopMaster.Areas.FrontEnd.Utility;
using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using System.Reflection;





namespace ShopMaster.Areas.FrontEnd.Controllers
{
    [Area("FrontEnd")]
    public class MemberController : Controller
    {
        protected readonly shopmasterdbContext _db;
        private readonly string _connectionString;

        public MemberController(shopmasterdbContext db, IConfiguration configuration)
        {
            _db = db;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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

          
            return View(member);
        }

        [HttpPost]
        
        public IActionResult EditProfile(BackEnd.Models.Member member)
        {
            
            var existingMember = _db.Members.Find(member.Id);
            if (existingMember == null)
            {
                return NotFound();
            }

            existingMember.Name = member.Name;
            existingMember.Email = member.Email;
            existingMember.Phone = member.Phone;
            existingMember.Address = member.Address;

           
            var tempCart = HttpContext.Session.Get<List<Areas.FrontEnd.ViewModelsF.Cart>>("tempCart") ?? new List<Areas.FrontEnd.ViewModelsF.Cart>();
            var ecoupon = _db.Ecoupons.Where(x => x.MemberId == member.Id).ToList();


            foreach (var i in tempCart)
            {
                if (i.Member == null)
                    i.Member = new Areas.FrontEnd.ViewModelsF.Member();

                i.Member.Name = member.Name;
                i.Member.Address = member.Address;
                i.Member.Phone = member.Phone;
                i.Member.Email = member.Email;
                i.MemberId = member.Id;                
                i.Member.PasswordHash = member.PasswordHash;
                i.Code = ecoupon.Select(x => x.Code).ToList();
            }
            
            HttpContext.Session.Set("tempCart", tempCart);
            HttpContext.Session.SetString("MemberId", member.Id.ToString());
            string? memberId = HttpContext.Session.GetString("MemberId");
            _db.SaveChanges();               

            return RedirectToAction("Index", "Member", new { Area = "FrontEnd"});                    

            
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

            // 購物車
            var tempCart = HttpContext.Session.Get<List<Areas.FrontEnd.ViewModelsF.Cart>>("tempCart") ?? new List<Areas.FrontEnd.ViewModelsF.Cart>();
            var ecoupon = _db.Ecoupons.Where(x => x.MemberId == member.Id).ToList();
            var memberType = _db.MemberTypes.Where(x => x.Id == member.MemberTypeId).ToList();
            
            if(tempCart.Count > 0)
            {
                foreach (var i in tempCart)
                {
                    if (i.Member == null)
                        i.Member = new Areas.FrontEnd.ViewModelsF.Member();

                    i.Member.Name = member.Name;
                    i.Member.Address = member.Address;
                    i.Member.Phone = member.Phone;
                    i.Member.Email = member.Email;
                    i.MemberId = member.Id;
                    i.Member.MemberType = memberType;
                    i.Member.PasswordHash = member.PasswordHash;
                    i.Code = ecoupon.Select(x => x.Code).ToList();
                }
            }
            else
            {
                // 假如先登入 先加入會員資料至購物車
                var newCart = new Areas.FrontEnd.ViewModelsF.Cart
                {
                    Member = new Areas.FrontEnd.ViewModelsF.Member
                    {
                        Name = member.Name,
                        Address = member.Address,
                        Phone = member.Phone,
                        Email = member.Email,    
                        MemberType = memberType,   
                        PasswordHash = member.PasswordHash
                    },
                    MemberId = member.Id,
                    Code = ecoupon.Select(x => x.Code).ToList()
                };

                tempCart.Add(newCart);
            }

            HttpContext.Session.Set("tempCart", tempCart);           

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

        [HttpGet]
        public IActionResult OrderList(long id)
        {
            //using (IDbConnection connection = new MySqlConnection(_connectionString))
            //{
            //    connection.Open();                

            //    string sql = @"SELECT * FROM MEMBER";
            //    try
            //    {
            //        var result = connection.Query<BackEnd.Models.Member>(sql).ToList();
            //    }
            //    catch(Exception ex)
            //    {                    
            //        Console.WriteLine(ex.Message);
            //    }

            if (id == 0)
            {
                return RedirectToAction("Index", "Member", new { Area = "FrontEnd" });
            }

            var member = _db.Members.FirstOrDefault(x => x.Id == id);
            if(member != null)
            {
                var product = _db.Products.Where(x => x.Id == member.Id).ToList();

                var order = _db.Orders
                                    .Include(x => x.OrderDetails)
                                    .ThenInclude(od => od.Product)
                                    .Include(x => x.Member)
                                    .Where(x => x.MemberId == member.Id)
                                    .ToList();


                var result = order.Select(x => new MemberOrderViewModel
                {
                    Id = id,
                    Name = x.Member.Name,
                    OrderDetail = x.OrderDetails.ToList(),
                    TotalAmount = x.TotalAmount,


                });
                return View(result);
            }

            
           


                return View();
        }


        public IActionResult Logout()
        {
            // 如果有購物車 清除基本資料            
            var tempCart = HttpContext.Session.Get<List<Areas.FrontEnd.ViewModelsF.Cart>>("tempCart") ?? new List<Areas.FrontEnd.ViewModelsF.Cart>();
            
            foreach(var i in tempCart)
            {
                i.Member.Name = "";
                i.Member.Address = "";
                i.Member.Phone = "";
                i.Member.Email = "";
                i.Code = new List<string>();
                i.Member.Id = 0;
            }
            
            HttpContext.Session.Set("tempCart", tempCart);
           
            HttpContext.Session.Remove("MemberId");

            return RedirectToAction("Index", "Home", new { Area = "FrontEnd" });
        }
    }
}
