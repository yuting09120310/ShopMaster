using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class EcouponEvent
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string CodePrefix { get; set; } = null!;

    public string CouponType { get; set; } = null!;

    public decimal Discount { get; set; }

    public decimal MinOrderAmount { get; set; }

    public int ExpiryDays { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Ecoupon> Ecoupons { get; set; } = new List<Ecoupon>();
}
