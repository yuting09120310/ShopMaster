using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModelsF;


namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class ProductsAll
    {
        //分類商品
        public List<IGrouping<int, Products>> ProductList { get; set; } = new List<IGrouping<int, Products>>(); 

        //喜愛商品
        public List<Products> ProductListLove { get; set; } = new List<Products>();

        //商品列表
        public  List<Products> ProductDetails { get; set; } = new List<Products>();

        //購物車
        public  List<Cart> ProductCart { get; set; } = new List<Cart>();
        

        public BackEnd.Models.Member? Member { get; set; }
        //public Member? Member { get; set; } 

        

    }
}
