using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Table("favorites")]
[Index("UserId", "ListingId", Name = "uq_fav_user_listing", IsUnique = true)]
public partial class Favorite
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("listing_id")]
    public int ListingId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("ListingId")]
    [InverseProperty("Favorites")]
    public virtual Listing Listing { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Favorites")]
    public virtual User User { get; set; } = null!;
}
