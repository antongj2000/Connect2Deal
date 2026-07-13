using System;
using System.Collections.Generic;

namespace Connect2Deal.Models;

public partial class UserRating
{
    public int Id { get; set; }

    public int TransactionId { get; set; }

    public int RaterId { get; set; }

    public int RatedUserId { get; set; }

    public int Score { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User RatedUser { get; set; } = null!;

    public virtual User Rater { get; set; } = null!;

    public virtual Transaction Transaction { get; set; } = null!;
}
