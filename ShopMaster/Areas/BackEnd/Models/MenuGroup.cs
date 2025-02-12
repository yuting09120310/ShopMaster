using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class MenuGroup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }
}
