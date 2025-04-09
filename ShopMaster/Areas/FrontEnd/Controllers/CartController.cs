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
using System.Diagnostics.Eventing.Reader;
using Newtonsoft.Json;
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
        
        Dictionary<long, long> countInputDy = new Dictionary<long, long>();
        Dictionary<long, long> countInputEdit = new Dictionary<long, long>();
        Dictionary<long, long> priceEdit = new Dictionary<long, long>();
        

        [HttpPost]
        public IActionResult AddToCart(int productId, string name, decimal price, string mainImage, int countInput)
        {
            var product = _db.Products.ToList();
            var cart = _db.Carts.ToList();
            var member = _db.Members.ToList();

            tempCart = HttpContext.Session.Get<List<Cart>>("tempCart") ?? new List<Cart>();

            if (tempCart == null)
            {

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

                HttpContext.Session.Set("tempCart", tempCart);

            }

            //HttpContext.Session.Set("tempCart", tempCart);

            //tempCart = HttpContext.Session.Get<List<Cart>>("tempCart") ?? new List<Cart>();

            var existingItem = tempCart.FirstOrDefault(c => c.ProductId == productId);

            tempCart.Add(new Cart
            {
                ProductId = productId,

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




            HttpContext.Session.Set("tempCart", tempCart);
           

            tempCart = HttpContext.Session.Get<List<Cart>>("tempCart") ?? new List<Cart>();
            var productsAll = new ProductsAll
                {
                    ProductCart = tempCart
                };

            //手動輸入數量

            Dictionary<long, long> totalInput = HttpContext.Session.Get<Dictionary<long, long>>("totalInput") ?? new Dictionary<long, long>();


            if (!totalInput.ContainsKey(productId))
            {
                totalInput.Add(productId, countInput);
            }
            else
            {
                totalInput[productId] += countInput;
            }
            

            HttpContext.Session.Set("totalInput", totalInput);

            countInputDy = HttpContext.Session.Get<Dictionary<long, long>>("totalInput") ?? new Dictionary<long, long>();
            
            return Json(new { success = true, message = "商品已加入購物車", cartItemCount = countInputDy.Values.Sum() });
            
        }


        [HttpPost]
        public IActionResult EditToAddCart(int productId, string name, decimal price, string mainImage, int countInput)
        {
            tempCart = HttpContext.Session.Get<List<Cart>>("tempCart") ?? new List<Cart>();

            Dictionary<long, long> totalInput = HttpContext.Session.Get<Dictionary<long, long>>("totalInput") ?? new Dictionary<long, long>();

            var existingItem = tempCart.FirstOrDefault(c => c.ProductId == productId);

            if (totalInput.ContainsKey(productId))
            {
                totalInput[productId] = countInput;
            }
            
            HttpContext.Session.Set("totalInput", totalInput);

            countInputDy = HttpContext.Session.Get<Dictionary<long, long>>("totalInput") ?? new Dictionary<long, long>();

            return Json(new { success = true, message = "商品已加入購物車", cartItemCount = countInputDy.Values.Sum() });

        }

        public IActionResult GetCart()
        {
            tempCart = HttpContext.Session.Get<List<Cart>>("tempCart") ?? new List<Cart>();

            var getCartTemp = tempCart.DistinctBy(p => p.ProductId).ToList();

            var productsAll = new ProductsAll
            {
                ProductCart = getCartTemp
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
        public ActionResult Create(List<Cart> cart)
        {
            countInputDy = HttpContext.Session.Get<Dictionary<long, long>>("totalInput") ?? new Dictionary<long, long>();

            tempCart = HttpContext.Session.Get<List<Cart>>("tempCart") ?? new List<Cart>();


            //先取 Product

            if (tempCart != null)
            {


                if (tempCart.Any(m => m.MemberId.HasValue))
                {

                }
                else
                {
                    //不用登入加入購物車

                    if (ModelState.IsValid)
                    {

                        //新增購物車至資料庫
                        var existingIds = _db.Carts.Select(c => c.Id).ToList();
                        int newId = 1;
                        while (existingIds.Contains(newId))
                        {
                            newId++;
                        }

                        var cartCreateDb = tempCart.Select((c, index) => new ShopMaster.Areas.BackEnd.Models.Cart
                        {

                            Id = newId++,
                            ProductId = c.ProductId,
                            MemberId = c.MemberId.HasValue ? c.MemberId.Value : 0

                        });


                        //商品數量
                        //var count = tempCart.GroupBy(x => x.ProductId)
                        //    .Select(g => new
                        //    {
                        //        productId = g.Key,
                        //        count = g.Count(),


                        //    }).ToList();

                        //ViewBag.count = count;

                        //自行輸入商品數量
                        ViewBag.totalInputDictionary = countInputDy;

                        HttpContext.Session.Set("totalInput", countInputDy);

                        var getCartTemp = tempCart.DistinctBy(p => p.ProductId).ToList();
                        // 商品金額
                        var price = getCartTemp.Select(c => c.CartItem.Select(p => new { c.ProductId, p.Price })).ToList();
                        Dictionary<long, decimal> priceDy = new Dictionary<long, decimal>();

                        decimal totalPrice = 0;

                        foreach (var c in countInputDy)
                        {
                            if (getCartTemp.Select(p => p.ProductId).ToList().Contains(c.Key))
                            {

                                // 獲取該 ProductId 的所有價格
                                var productPrices = getCartTemp
                                    .Where(p => p.ProductId == c.Key)  // 篩選出匹配的 ProductId
                                    .SelectMany(p => p.CartItem.Select(x => x.Price))  // 獲取該 ProductId 對應的所有價格
                                    .ToList();

                                // 計算總價格
                                foreach (var pp in productPrices)
                                {
                                    totalPrice = pp * (decimal)c.Value;
                                }

                                if (!priceDy.ContainsKey(c.Key))
                                {
                                    priceDy.Add(c.Key, totalPrice);
                                }


                            }

                        }

                        ViewBag.totalPrice = priceDy;

                        HttpContext.Session.Set("priceDy", priceDy);

                        var productsAll = new ProductsAll
                        {
                            ProductCart = getCartTemp

                        };

                        _db.Carts.AddRange(cartCreateDb);
                        _db.SaveChanges();


                        return View(productsAll);
                    }
                }
                ;
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
        public ActionResult Edit(int id )
        {

            return View();
        }

        // POST: CartController/Edit/5
        [HttpPost]
        
        public ActionResult Edit(int id, string productId, int quantity)
        {
           
                return View();
            
        }

        // GET: CartController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CartController/Delete/5
        [HttpPost]
       
        public ActionResult Delete(long deleteId, int deleteCount)
        {
            try
            {
                tempCart = HttpContext.Session.Get<List<Cart>>("tempCart") ?? new List<Cart>();

                countInputDy = HttpContext.Session.Get<Dictionary<long, long>>("totalInput") ?? new Dictionary<long, long>();


                var item = tempCart.FirstOrDefault(c => c.ProductId == deleteId);

                if (tempCart != null)
                {
                    tempCart.Remove(item); // 移除找到的項目
                }

                if (countInputDy.ContainsKey(deleteId))
                {
                    countInputDy.Remove(deleteId);
                }


                // 更新 Session
                HttpContext.Session.Set("tempCart", tempCart);

                HttpContext.Session.Set("totalInput", countInputDy);


                return Json(new { success = true, message = "商品已刪除", id= deleteId, cartItemCount = countInputDy.Values.Sum() });
            }
            catch
            {
                return View();
            }
        }
    }
}
