using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class Member
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Avatar { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateOnly? Birthday { get; set; }

    public int MemberTypeId { get; set; }

    public int Active { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual MemberType MemberType { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
