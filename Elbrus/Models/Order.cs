using System;
using System.Collections.Generic;

namespace Elbrus.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string? OrderCode { get; set; }

    public DateOnly? StartDate { get; set; }

    public TimeOnly? Time { get; set; }

    public string? Status { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? ClientId { get; set; }

    public virtual Client? Client { get; set; }
}
