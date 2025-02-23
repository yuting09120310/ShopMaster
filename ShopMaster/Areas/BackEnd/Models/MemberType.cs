using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class MemberType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Discount { get; set; }

    public decimal UpgradeThreshold { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
