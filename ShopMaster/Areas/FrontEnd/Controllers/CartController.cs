using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModelsF;
using ShopMaster.Areas.FrontEnd.Utility;
using Cart = ShopMaster.Areas.FrontEnd.ViewModelsF.Cart;
using Microsoft.EntityFrameworkCore;
using Member = ShopMaster.Areas.FrontEnd.ViewModelsF.Member;
using System.Collections.Generic;
using System.Xml.Linq;
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

        public object Name { get; private set; }

        [HttpPost]
        public IActionResult AddToCart(int productId, string name, decimal price,string mainImage )
        {
            var product = _db.Products.ToList();
            var cart = _db.Carts.ToList();
            var member = _db.Members.ToList();

            //先組暫時的購物車 
            tempCart = cart.Join(product,
                c => c.ProductId,
                p => p.Id,
                (c, p) => new { c, p })
                .Join(member,
                cp => cp.p.Id,
                m => m.Id,
                (cp, m) 
                => new ViewModelsF.Cart 
                { 
                    //Id = cp.c.Id,
                    ProductId = cp.p.Id,
                    MemberId = m.Id,
                    Member = new List<Member>
                    {
                        new Member
                        {
                            Name = m.Name,                            
                        }
                    },
                    CartItem = new List<Products>
                    {
                        new Products
                        {
                            Name = cp.p.Name,
                            Price = cp.p.Price,
                            MainImage = cp.p.MainImage,
                            
                        
                        }
                    }


                }).ToList();

            
             tempCart = HttpContext.Session.Get<List<Cart>>("tempCart") ?? new List<Cart>();
           
            var existingItem = tempCart.FirstOrDefault(c => c.ProductId == productId);
            
            if (existingItem != null)
            {
              

            }
            else
            {
                tempCart.Add(new Cart { ProductId = productId,

                    CartItem = new List<Products>
                    {
                        new Products
                        { 
                            Name = name,
                            Price = price,
                            MainImage = mainImage                           
                            
                        }
                        
                    }
                   
                });
                
            }


            //var tempCartTwo = tempCart.Select(x => x.ProductId).Distinct().ToList();

            HttpContext.Session.Set("tempCart", tempCart);



          
            var productsAll = new ProductsAll
            {
                ProductCart = tempCart
            };
            

            return Json(new { success = true, message = "商品已加入購物車", cartItemCount = tempCart.Count });
        }

        public IActionResult GetCart()
        {
            tempCart = HttpContext.Session.Get<List<Cart>>("tempCart") ?? new List<Cart>();

            var productsAll = new ProductsAll
            {
                ProductCart = tempCart
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
        public ActionResult Create(List<Cart> cart )
        {
            tempCart = HttpContext.Session.Get<List<Cart>>("tempCart") ?? new List<Cart>();
            //先取 Product

            if (tempCart.Any(m=>m.MemberId.HasValue))
            {

            }
            else
            {
                //不用登入加入購物車

                if (ModelState.IsValid)
                {
                    var productsAll = new ProductsAll
                    {
                        ProductCart = tempCart               

                    };

                    var existingIds = _db.Carts.Select(c => c.Id).ToList();
                    int newId = 1; 
                    while (existingIds.Contains(newId))
                    {
                        newId++; 
                    }

                    var cartCreate = productsAll.ProductCart.Select((c,index) => new ShopMaster.Areas.BackEnd.Models.Cart
                    {

                        Id = newId++,
                        ProductId = c.ProductId,
                        MemberId = c.MemberId.HasValue ? c.MemberId.Value : 0


                    });

                    

                    _db.Carts.AddRange(cartCreate);
                    _db.SaveChanges();


                    return View(productsAll);
                }                
            };



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
