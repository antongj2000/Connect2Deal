using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Table("categories")]
[Index("Slug", Name = "categories_slug_key", IsUnique = true)]
public partial class Category
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("slug")]
    [StringLength(60)]
    public string Slug { get; set; } = null!;

    [Column("parent_id")]
    public int? ParentId { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    [InverseProperty("Category")]
    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual Category? Parent { get; set; }
}
