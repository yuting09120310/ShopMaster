using Microsoft.AspNetCore.Mvc.Rendering;
using ShopMaster.Areas.FrontEnd.EnumUtility;
using static ShopMaster.Areas.FrontEnd.EnumUtility.PaymentEnum;

namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public  class PayInfo
    {
        public Pay? SelectedPaymentMethod { get; set; }

        // 下拉選單項目
        public List<SelectListItem> PaymentMethodList { get; set; } = null!;

    }
}
