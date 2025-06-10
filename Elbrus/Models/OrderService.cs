using System;
using System.Collections.Generic;

namespace Elbrus.Models;

public partial class OrderService
{
    public int OrderId { get; set; }

    public int ServiceId { get; set; }

    public int RentTime { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
