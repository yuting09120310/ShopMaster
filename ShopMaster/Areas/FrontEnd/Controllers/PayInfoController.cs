using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModelsF;
using ShopMaster.Areas.FrontEnd.Services;
using static ShopMaster.Areas.FrontEnd.EnumUtility.PaymentEnum;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopMaster.Areas.FrontEnd.EnumUtility;

namespace ShopMaster.Areas.FrontEnd.Controllers
{
    public class PayInfoController : Controller
    {

        private readonly shopmasterdbContext _db;

        public PayInfoController(shopmasterdbContext db)
        {
            _db = db;            
        }

        public IActionResult Pavement()
        {
            var payInfo = _db.PayInfos.ToList();

            // 建立 Enum 對應的下拉選單資料
            var paymentMethodList = Enum.GetValues(typeof(Pay))
                           .Cast<Pay>()
                           .Select(x => new SelectListItem
                           {
                               Value = ((int)x).ToString(),
                               Text = x.ToString()

                           }).ToList();

            // 預設信用卡
            Pay selectedPaymentMethod = Pay.信用卡;

            if (payInfo.Any())
            {
                selectedPaymentMethod = (Pay)payInfo.First().Id;
            }

            var model = new FrontEnd.ViewModelsF.PayInfo
            {
                SelectedPaymentMethod = selectedPaymentMethod,
                PaymentMethodList = paymentMethodList
            };

            return View(model);

        }
    }    
}
