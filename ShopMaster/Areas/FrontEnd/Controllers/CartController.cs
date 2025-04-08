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
            public string SpecId { get; set; }
            public int Quantity { get; set; }
        }


        [HttpPost]
        public ActionResult AddCart(long productId, string specId, int quantity)
        {
            if (productId <= 0 || string.IsNullOrEmpty(specId) || quantity <= 0)
            {
                return BadRequest("Invalid product ID, spec ID, or quantity.");
            }

            // 確認會員是否已登入
            string? memberId = HttpContext.Session.GetString("MemberId");
            if (string.IsNullOrEmpty(memberId))
            {
                return Unauthorized("請先登入會員。");
            }

            // 檢查商品是否存在
            var product = _db.Products.Include(p => p.ProductSpecs)
                                      .FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound("商品不存在。");
            }

            // 檢查規格是否存在
            var spec = product.ProductSpecs.FirstOrDefault(s => s.Id.ToString() == specId);
            if (spec == null)
            {
                return NotFound("商品規格不存在。");
            }

            // 新增購物車項目
            var cart = new Cart
            {
                MemberId = Convert.ToInt64(memberId),
                ProductId = productId,
                ProductSpec = spec.Id.ToString(),
                Quantity = quantity
            };

            _db.Carts.Add(cart);
            _db.SaveChanges();

            return Ok(new { message = "商品已成功加入購物車。" });
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
