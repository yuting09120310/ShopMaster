using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class Product
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int? TypeId { get; set; }

    public bool? Publish { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ProductType? Type { get; set; }
}
