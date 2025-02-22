using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.BackEnd.ViewModels
{
    public class EcouponDistributeViewModel
    {
        public long EventId { get; set; } // 優惠活動 ID
        public string EventName { get; set; } = null!; // 活動名稱
        public List<Member> Members { get; set; } = new List<Member>(); // 可選會員列表
    }
}
