namespace ShopMaster.Areas.BackEnd.ViewModels
{
    public class OrderDetailViewModel
    {
        public long OrderId { get; set; }
        public string MemberName { get; set; } = null!;
        public string MemberEmail { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; } 
        public decimal MemberTotalDiscount { get; set; } 
        public decimal CouponTotalDiscount { get; set; }
        public string PaymentType { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; } = new();
        public List<EcouponViewModel> UsedCoupons { get; set; } = new(); 
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class EcouponViewModel
    {
        public string Code { get; set; } = null!;
        public decimal Discount { get; set; } 
    }
}
