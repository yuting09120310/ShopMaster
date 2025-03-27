using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModelsF;
using ShopMaster.Areas.FrontEnd.Utility;
using Cart = ShopMaster.Areas.FrontEnd.ViewModelsF.Cart;
using Microsoft.EntityFrameworkCore;
using Member = ShopMaster.Areas.FrontEnd.ViewModelsF.Member;
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
        // GET: CartController
        public ActionResult Index()
        {           

                return View();
        }


        List<Cart> tempCart = new List<Cart>(); 
        [HttpPost]
        public IActionResult AddToCart(int productId, string name, decimal price,string mainImage )
        {
            var product =  _db.Products.ToList();
            var cartDb = _db.Carts.ToList();
            var member = _db.Members.ToList();

            //先組暫時的購物車 
            tempCart = cartDb.Join(product, c => c.ProductId, p => p.Id, (c, p) => new { c, p })
                .Join(member, cp => cp.p.Id, m => m.Id,
                (cp, m) 
                => new ViewModelsF.Cart 
                { 
                    Id = cp.c.Id,
                    ProductId = cp.p.Id,
                    MemberId = m.Id,
                    Member = new List<Member>
                    {
                        new Member
                        {
                            Name = m.Name,                            
                        }
                    },
                    CartItem = new List<Product>
                    {
                        new Product
                        {
                            Name = cp.p.Name,
                            Price = cp.p.Price,
                            MainImage = cp.p.MainImage                    
                        
                        }
                    }


                }).ToList();

            //var cart = HttpContext.Session.Get<List<Cart>>("Cart") ?? new List<Cart>();
             tempCart = HttpContext.Session.Get<List<Cart>>("tempCart") ?? new List<Cart>();

            //var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
            var existingItem = tempCart.FirstOrDefault(c => c.ProductId == productId);
            if (existingItem != null)
            {
               



            }
            else
            {
                tempCart.Add(new Cart { ProductId = productId,

                    CartItem = new List<Product>
                    {
                        new Product
                        {
                            Name = name,
                            Price = price,
                            MainImage = mainImage                            

                        }
                    }

                });
            }

            HttpContext.Session.Set("tempCart", tempCart);

            var productsAll = new ProductsAll
            {
                productCart = tempCart
            };

            return Json(new { success = true, message = "商品已加入購物車", cartItemCount = tempCart.Count });
        }

        public IActionResult GetCart()
        {
            tempCart = HttpContext.Session.Get<List<Cart>>("tempCart") ?? new List<Cart>();

            var productsAll = new ProductsAll
            {
                productCart = tempCart
            };

            return PartialView("_CartPartial", productsAll);
        }

        // GET: CartController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CartController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CartController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? memberId, Product product)
        {
            
            //先取 Product

            if (memberId.HasValue)
            {


            }
            else
            {
                //不用登入加入購物車

                if (ModelState.IsValid)
                {

                }
                

            }



            //try
            //{
            //    return RedirectToAction(nameof(Index));
            //}
            //catch
            //{
            //    return View();
            //}
            return View();
        }

        // GET: CartController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CartController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CartController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CartController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
