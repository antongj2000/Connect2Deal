using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

public partial class listing
{
    [Key]
    public int id { get; set; }

    public int user_id { get; set; }

    public int category_id { get; set; }

    [StringLength(120)]
    public string title { get; set; } = null!;

    [StringLength(2000)]
    public string description { get; set; } = null!;

    [Precision(12, 2)]
    public decimal? price { get; set; }

    [StringLength(80)]
    public string? city { get; set; }

    [StringLength(20)]
    public string status { get; set; } = null!;

    public int view_count { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime expires_at { get; set; }

    [ForeignKey("category_id")]
    [InverseProperty("listings")]
    public virtual category category { get; set; } = null!;

    [InverseProperty("listing")]
    public virtual transaction? transaction { get; set; }

    [ForeignKey("user_id")]
    [InverseProperty("listings")]
    public virtual user user { get; set; } = null!;
}
