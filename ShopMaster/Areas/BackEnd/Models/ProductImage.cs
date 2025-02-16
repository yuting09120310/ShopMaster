using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class ProductImage
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int? SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;
}
