using ShopMaster.Areas.BackEnd.Models;
namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class Cart
    {
        public long Id { get; set; }

        public long? MemberId { get; set; }

        public long? ProductId { get; set; }
        
        public List<string> Code { get; set; } = new List<string>();

        public virtual List<Products> CartItem { get; set; } = new List<Products>();

        //public virtual ICollection<Member> Member { get; set; } = new List<Member>();

        private Member _member = new Member();
        public Member Member
        {
            get => _member;

            set
            {
                if (value == null || string.IsNullOrWhiteSpace(value.Name))
                {
                    _member = new Member
                    {
                        Name = "非會員",     
                                            
                    };                   
                }
                else
                {
                    _member = value;
                }
            }
        }
    }
}
