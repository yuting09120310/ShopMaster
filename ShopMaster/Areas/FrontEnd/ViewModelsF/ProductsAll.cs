using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModelsF;


namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class ProductsAll
    {
        public List<IGrouping<int, Products>> ProductList { get; set; }
        public List<Products> ProductListLove { get; set; }
        public  List<Products> ProductDetails { get; set; } = new List<Products>();
}
}
