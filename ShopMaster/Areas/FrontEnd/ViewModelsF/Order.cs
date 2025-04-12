using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class Order
    {
        public long Id { get; set; }

        public long MemberId { get; set; }

        public int MemberTypeId { get; set; }

        public decimal TotalAmount { get; set; }

        public int Status { get; set; }

        public int PaymentType { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Ecoupon> Ecoupons { get; set; } = new List<Ecoupon>();

        public virtual Member Member { get; set; } = null!;

        public virtual MemberType MemberType { get; set; } = null!;

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public virtual PayInfo PaymentTypeNavigation { get; set; } = null!;
    }
}
