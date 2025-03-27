using ShopMaster.Areas.BackEnd.Models;
namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class Cart
    {
        public long Id { get; set; }

        public long? MemberId { get; set; }

        public long? ProductId { get; set; }

        public virtual ICollection<Product> CartItem { get; set; } = new List<Product>();
        public virtual ICollection<Member> Member { get; set; } = new List<Member>();
    }
}
