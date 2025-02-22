using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class PayInfo
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
