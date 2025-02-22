using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class Ecoupon
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public long EcouponEventId { get; set; }

    public long MemberId { get; set; }

    public long? OrderId { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public int? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual EcouponEvent EcouponEvent { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;

    public virtual Order? Order { get; set; }
}
