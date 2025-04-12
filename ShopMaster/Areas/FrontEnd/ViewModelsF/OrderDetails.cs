using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class OrderDetails
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public long ProductId { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal FinalPrice { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual Order Order { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}
