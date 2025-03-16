using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class Product
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string? MainImage { get; set; }

        //副標題
        public string? Description { get; set; }

        public string? Content { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int? TypeId { get; set; }

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public virtual ProductType? Type { get; set; }
    }
}
