﻿using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class Products
    {
        public long Id { get; set; }

        //商品名稱
        public string Name { get; set; } = null!;

        public string TypeName{ get; set; } = null!;

        public string? MainImage { get; set; }

        public string? ProductImage { get; set; }

        //副標題
        public string? Description { get; set; }

        public string? Content { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        

        public int? TypeId { get; set; }

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        
    }
}
