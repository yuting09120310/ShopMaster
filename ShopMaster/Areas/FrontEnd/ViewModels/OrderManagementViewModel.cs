using System;

namespace ShopMaster.Areas.FrontEnd.ViewModels
{
    public class OrderManagementViewModel
    {
        public List<OrderViewModel> InProgressOrders { get; set; } = new List<OrderViewModel>();
        public List<OrderViewModel> CompletedOrders { get; set; } = new List<OrderViewModel>();
    }

    public class OrderViewModel
    {
        public long OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
