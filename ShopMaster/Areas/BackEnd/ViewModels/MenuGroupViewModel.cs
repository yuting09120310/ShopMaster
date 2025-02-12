using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.BackEnd.ViewModels
{
    public class MenuGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<MenuSub> MenuSubs { get; set; } = new List<MenuSub>();
    }

}
