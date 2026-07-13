using System;
using System.Collections.Generic;

namespace Connect2Deal.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public int ListingId { get; set; }

    public int SellerId { get; set; }

    public int BuyerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Buyer { get; set; } = null!;

    public virtual Listing Listing { get; set; } = null!;

    public virtual User Seller { get; set; } = null!;

    public virtual ICollection<UserRating> UserRatings { get; set; } = new List<UserRating>();
}
