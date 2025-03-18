namespace ShopMaster.Areas.BackEnd.ViewModels
{
    public class DashBoard
    {
        // 當月訂單數
        public int MonthOrderCount { get; set; }

        // 當月銷貨金額
        public decimal MonthTotalAmount { get; set; }

        // 當年訂單數
        public int YearOrderCount { get; set; }

        // 當年銷貨金額
        public decimal YearTotalAmount { get; set; }
    }
}
