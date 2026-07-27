using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Table("reports")]
public partial class Report
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("listing_id")]
    public int ListingId { get; set; }

    [Column("reporter_id")]
    public int ReporterId { get; set; }

    [Column("reason")]
    [StringLength(255)]
    public string? Reason { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("ListingId")]
    [InverseProperty("Reports")]
    public virtual Listing Listing { get; set; } = null!;

    [ForeignKey("ReporterId")]
    [InverseProperty("Reports")]
    public virtual User Reporter { get; set; } = null!;
}
