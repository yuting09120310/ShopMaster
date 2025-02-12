using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class Rule
{
    public int GroupId { get; set; }

    public int MenuId { get; set; }

    public bool? CanCreate { get; set; }

    public bool? CanRead { get; set; }

    public bool? CanUpdate { get; set; }

    public bool? CanDelete { get; set; }
}
