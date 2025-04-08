using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopMaster.Areas.BackEnd.Models;
using Microsoft.EntityFrameworkCore;
namespace ShopMaster.Areas.FrontEnd.Controllers
{
    [Area("FrontEnd")]
    public class CartController : Controller
    {
        private readonly shopmasterdbContext _db;

        public CartController(shopmasterdbContext db)
        {
            _db = db;
        }

        public class AddCartRequest
        {
            public long ProductId { get; set; }
            public string Color { get; set; }
            public int Quantity { get; set; }
        }


        [HttpPost]
        public ActionResult AddCart([FromBody] AddCartRequest request)
        {
            if (request == null || request.ProductId <= 0 || request.Quantity <= 0)
            {
                return BadRequest("Invalid product ID, color, or quantity.");
            }

            // 假設有一個 Cart 物件來表示購物車
            var cart = new Cart
            {
                ProductId = request.ProductId,
            };

            // 將購物車項目添加到數據庫
            _db.Carts.Add(cart);
            _db.SaveChanges();

            return Ok("Product added to cart successfully.");
        }


        [HttpGet]
        public ActionResult GetCartPartial()
        {
            string? memberId = HttpContext.Session.GetString("MemberId");

            var cartItems = _db.Carts.Where(c => c.MemberId == Convert.ToInt64(memberId)).ToList();

            foreach(var cartItem in cartItems)
            {
                cartItem.Product = _db.Products
                    .Where(p => p.Id == cartItem.ProductId)
                    .FirstOrDefault();
            }

            return PartialView("_CartPartial", cartItems);
        }
    }
}
