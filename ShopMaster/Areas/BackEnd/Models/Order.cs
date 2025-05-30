﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopMaster.Areas.BackEnd.Models;


public partial class Order
{
    
    public long Id { get; set; }

    public long MemberId { get; set; }

    public int MemberTypeId { get; set; }

    public decimal TotalAmount { get; set; }

    public int Status { get; set; }

    public int PaymentType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Ecoupon> Ecoupons { get; set; } = new List<Ecoupon>();

    public virtual Member Member { get; set; } = null!;

    public virtual MemberType MemberType { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual PayInfo PaymentTypeNavigation { get; set; } = null!;
}
