using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModelsF;
using ShopMaster.Areas.FrontEnd.Utility;
using static ShopMaster.Areas.FrontEnd.EnumUtility.PaymentEnum;
using Microsoft.EntityFrameworkCore;
using System;

namespace ShopMaster.Areas.FrontEnd.Controllers
{
    [Area("FrontEnd")]
    public class OrderController : Controller
    {
        private readonly shopmasterdbContext _db;
        List<Areas.FrontEnd.ViewModelsF.Cart> tempCart = new List<Areas.FrontEnd.ViewModelsF.Cart>();

        public OrderController(shopmasterdbContext db)
        {
            _db = db;
        }

        // GET: OrderController
        public ActionResult Index()
        {
            return View();
        }

        // GET: OrderController/Details/5
        public ActionResult Details(int id, int extraId)
        {            
            var member = _db.Members.FirstOrDefault(x => x.Id == id);
            MemberOrderViewModel result = new MemberOrderViewModel();

            if (member != null)
            {
                var order = _db.Orders.FirstOrDefault(x => x.Id == extraId && x.MemberId == member.Id);                

                if(order != null)
                {
                    var orderDetail = _db.OrderDetails
                              .Where(od => od.OrderId == order.Id)
                              .ToList();

                    foreach (var p in orderDetail)
                    {
                        var product = _db.Products.FirstOrDefault(x => x.Id == p.ProductId);

                         result = new MemberOrderViewModel
                        {
                            Name = member.Name,
                            OrderDetail = orderDetail,                            
                            TotalAmount = order.TotalAmount,

                        };
                    }

                    return View(result);
                } 

            }

            return View();


            
        }

        








        // GET: OrderController/Create
        public ActionResult Create()
        {
            // 取付款方式下拉選單資料
            var payInfo = _db.PayInfos.ToList();

            // 判斷會員是否登入
            string? memberId = HttpContext.Session.GetString("MemberId");
            var member = _db.Members.Find(Convert.ToInt64(memberId));
            bool isMember = member != null; 

            var paymentMethodList = Enum.GetValues(typeof(Pay))
                           .Cast<Pay>()
                           .Where(x=>isMember || x != Pay.貨到付款)
                           .Select(x => new SelectListItem
                           {
                               Value = ((int)x).ToString(),
                               Text = x.ToString()

                           }).ToList();

            // 預設信用卡
            //Pay selectedPaymentMethod = Pay.信用卡;

            //if (payInfo.Any())
            //{
            //    selectedPaymentMethod = (Pay)payInfo.First().Id;
            //}
            //

            paymentMethodList.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "請選擇付款方式"
            });

            var model = new FrontEnd.ViewModelsF.PayInfo
            {
                PaymentMethodList = paymentMethodList
            };

            // 取購物車
            var tempCart = HttpContext.Session.Get<List<Areas.FrontEnd.ViewModelsF.Cart>>("tempCart") ?? new List<Areas.FrontEnd.ViewModelsF.Cart>();
            Dictionary<long, long> totalInput = HttpContext.Session.Get<Dictionary<long, long>>("totalInput") ?? new Dictionary<long, long>();
            Dictionary<long, decimal> price = HttpContext.Session.Get<Dictionary<long, decimal>>("priceDy") ?? new Dictionary<long, decimal>();

            var getCartTemp = tempCart.DistinctBy(p => p.ProductId).ToList();

            var orderViewModel = new OrderViewModel
            {                
                cartList = getCartTemp,
                total = totalInput,
                price = price,
                payInfo = model
            };

            return View(orderViewModel);
        }

        // POST: OrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel model)
        {            
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var member = model.cartList.First().Member;
                var product = model.cartList.Where(x=>x.ProductId != null).ToList();

                var quanity = model.total.ToList();                

                //訂單
                var order = new Areas.BackEnd.Models.Order
                {
                    MemberId = model.cartList.Select(x => x.MemberId).FirstOrDefault() ?? 100000,
                    MemberTypeId = member.MemberTypeId,
                    Status = 1,
                    PaymentType = 2,
                    TotalAmount = 0
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();               
                
                // 訂單明細
                List<OrderDetail> orderDetails = new List<OrderDetail>();                

                foreach (var c in product)
                {
                    var productId = c.ProductId ?? 0;
                    var code = string.Join("",c.Code.ToList());
                    decimal shipping = 60;              

                    // 折扣金額
                    var discountRole = new List<(string rule, decimal discountAmount)>()
                    {
                        ("FREESHIP", 0),
                        ("WELCOME100", 100),
                        ("ANNIV", 500),
                        ("DOUBLE11", 200),
                        ("請選擇優惠券", 1),
                    };

                    decimal discountAmount = discountRole.
                                             Where(r => code.StartsWith(r.rule))
                                             .Select(r => r.discountAmount)
                                             .FirstOrDefault();     

                   if(discountAmount == 1m)
                   {
                        shipping = 60;
                        discountAmount = 0;
                   }
                   else if (discountAmount == 0m && c.MemberId != 100000)
                   {
                        shipping = 0;
                   }
                    else
                   {
                        shipping = 60;
                   }


                        model.price.TryGetValue(productId, out var amount);
                    decimal originalPrice = 0;

                    // 單價
                    if (model.total.TryGetValue(productId, out var quantity))
                    {
                        originalPrice = amount / (decimal)quantity;
                    }
                    else
                    {
                        originalPrice = 0;
                    }

                    orderDetails.Add(new Areas.BackEnd.Models.OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = productId,
                        Quantity = Convert.ToInt32(quantity),                        
                        OriginalPrice = originalPrice,
                        FinalPrice = amount,
                        SubTotal = amount - discountAmount + shipping

                    });                    
                }

                decimal totalAmount = orderDetails.Sum(x => x.SubTotal);

                // 更新訂單金額
                order.TotalAmount = totalAmount;

                _db.OrderDetails.AddRange(orderDetails);
                _db.Orders.Update(order);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("OrderOk", "Order", new {Area="FrontEnd"});
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, "發生錯誤：" + ex.Message);
               
                return View(model);
            }
        }

        public ActionResult OrderOk()
        {
            // 找會員
            string? memberId = HttpContext.Session.GetString("MemberId");     
            

            BackEnd.Models.Order? order = new BackEnd.Models.Order();

            if (!string.IsNullOrEmpty(memberId))
            {
                //找訂單
                order = _db.Orders.Where(x => x.MemberId == long.Parse(memberId))
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefault();

                ViewBag.memberId = memberId;
            }
            else
            {
                //找訂單
                 order = _db.Orders.Where(x => x.MemberId == 100000)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefault();

                ViewBag.memberId =order.MemberId;
            }
                 
            
            ViewBag.orderID = order.Id;

            return View();
        }

        // GET: OrderController/Edit/5
        public ActionResult Edit(int id)
        {
            



            return View();
        }

        // POST: OrderController/Edit/5
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

        // GET: OrderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OrderController/Delete/5
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
