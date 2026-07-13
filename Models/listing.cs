using System;
using System.Collections.Generic;


namespace Connect2Deal.Models;
public partial class Listing
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal? Price { get; set; }

    public string Status { get; set; } = null!;

    public int ViewCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public int LocationId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;

    public virtual Transaction? Transaction { get; set; }

    public virtual User User { get; set; } = null!;
}
