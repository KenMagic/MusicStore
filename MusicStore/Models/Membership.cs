using System;
using System.Collections.Generic;

namespace MusicStore.Models;

public partial class Membership
{
    public int MembershipId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? MembershipPlan { get; set; }

    public decimal? Amount { get; set; }

    public string? Status { get; set; }

    public virtual Customer? Customer { get; set; }
}
