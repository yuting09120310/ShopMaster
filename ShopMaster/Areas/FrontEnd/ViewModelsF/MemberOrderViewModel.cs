using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class MemberOrderViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public List<OrderDetail> OrderDetail { get; set; } = new List<OrderDetail>();

        

    }
}
