using ShopMaster.Areas.BackEnd.Models;
namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class Cart
    {
        public long Id { get; set; }

        public long? MemberId { get; set; }

        public long? ProductId { get; set; }

        public virtual ICollection<Product> ProductCart { get; set; } = new List<Product>();
    }
}
