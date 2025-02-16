using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class Product
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? MainImage { get; set; }

    public string? Description { get; set; }

    public string? Content { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int? TypeId { get; set; }

    public int? Publish { get; set; }

    public int? Views { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ProductType? Type { get; set; }
}
