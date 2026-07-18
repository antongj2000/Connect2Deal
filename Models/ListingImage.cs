using System;
using System.Collections.Generic;

namespace Connect2Deal.Models;

public partial class ListingImage
{
    public int Id { get; set; }

    public int ListingId { get; set; }

    public string ImagePath { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }

    public virtual Listing Listing { get; set; } = null!;
}
