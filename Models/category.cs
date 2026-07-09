using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Index("slug", Name = "categories_slug_key", IsUnique = true)]
public partial class category
{
    [Key]
    public int id { get; set; }

    [StringLength(50)]
    public string name { get; set; } = null!;

    [StringLength(60)]
    public string slug { get; set; } = null!;

    public int? parent_id { get; set; }

    [InverseProperty("parent")]
    public virtual ICollection<category> Inverseparent { get; set; } = new List<category>();

    [InverseProperty("category")]
    public virtual ICollection<listing> listings { get; set; } = new List<listing>();

    [ForeignKey("parent_id")]
    [InverseProperty("Inverseparent")]
    public virtual category? parent { get; set; }
}
