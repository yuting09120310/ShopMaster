using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class MenuSub
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public string Name { get; set; } = null!;

    public string Route { get; set; } = null!;

    public string? Icon { get; set; }

    public int? SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }
}
