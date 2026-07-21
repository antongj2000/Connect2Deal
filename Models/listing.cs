using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Table("listings")]
public partial class Listing
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("title")]
    [StringLength(120)]
    public string Title { get; set; } = null!;

    [Column("description")]
    [StringLength(2000)]
    public string Description { get; set; } = null!;

    [Column("price")]
    [Precision(12, 2)]
    public decimal? Price { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Column("view_count")]
    public int ViewCount { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("location_id")]
    public int LocationId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Listings")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Listing")]
    public virtual ICollection<ListingImage> ListingImages { get; set; } = new List<ListingImage>();

    [ForeignKey("LocationId")]
    [InverseProperty("Listings")]
    public virtual Location Location { get; set; } = null!;

    [InverseProperty("Listing")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [InverseProperty("Listing")]
    public virtual Transaction? Transaction { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Listings")]
    public virtual User User { get; set; } = null!;
}
