using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Table("locations")]
[Index("Slug", Name = "locations_slug_key", IsUnique = true)]
public partial class Location
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(80)]
    public string Name { get; set; } = null!;

    [Column("slug")]
    [StringLength(80)]
    public string Slug { get; set; } = null!;

    [Column("parent_id")]
    public int? ParentId { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<Location> InverseParent { get; set; } = new List<Location>();

    [InverseProperty("Location")]
    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual Location? Parent { get; set; }
}
