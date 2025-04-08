using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class ProductSpec
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public string SpecText { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
