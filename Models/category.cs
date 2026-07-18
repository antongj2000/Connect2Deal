using System;
using System.Collections.Generic;

namespace Connect2Deal.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int? ParentId { get; set; }

    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();

    public virtual Category? Parent { get; set; }
}
