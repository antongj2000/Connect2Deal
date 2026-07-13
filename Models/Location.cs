using System;
using System.Collections.Generic;

namespace Connect2Deal.Models;

public partial class Location
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int? ParentId { get; set; }

    public virtual ICollection<Location> InverseParent { get; set; } = new List<Location>();

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();

    public virtual Location? Parent { get; set; }
}
