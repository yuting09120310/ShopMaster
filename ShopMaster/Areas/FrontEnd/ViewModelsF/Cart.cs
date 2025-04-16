using ShopMaster.Areas.BackEnd.Models;
namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class Cart
    {
        public long Id { get; set; }

        public long? MemberId { get; set; }

        public long? ProductId { get; set; }
        
        public List<string> Code { get; set; } = new List<string>();

        public virtual ICollection<Products> CartItem { get; set; } = new List<Products>();

        //public virtual ICollection<Member> Member { get; set; } = new List<Member>();

        public Member Member = null!;

    }
}
