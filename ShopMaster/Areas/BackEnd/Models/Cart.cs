using ShopMaster.Areas.FrontEnd.ViewModelsF;
using System;
using System.Collections.Generic;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class Cart
{
    public long Id { get; set; }

    public long? MemberId { get; set; }

    public long? ProductId { get; set; }

    
}
