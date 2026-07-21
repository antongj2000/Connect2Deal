using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Table("listing_images")]
public partial class ListingImage
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("listing_id")]
    public int ListingId { get; set; }

    [Column("image_path")]
    [StringLength(255)]
    public string ImagePath { get; set; } = null!;

    [Column("is_primary")]
    public bool IsPrimary { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [ForeignKey("ListingId")]
    [InverseProperty("ListingImages")]
    public virtual Listing Listing { get; set; } = null!;
}
