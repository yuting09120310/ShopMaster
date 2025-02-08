using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class Admin
{
    public long Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int GroupId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual AdminGroup Group { get; set; } = null!;
}
