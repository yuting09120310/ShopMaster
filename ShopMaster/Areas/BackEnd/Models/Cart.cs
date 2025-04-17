using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class Cart
{
    public long Id { get; set; }

    public long? MemberId { get; set; }

    public long ProductId { get; set; }

    public int Quantity { get; set; }

    public string ProductSpec { get; set; } = null!;

    public virtual Product Product { get; set; }
}
