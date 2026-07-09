using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Index("transaction_id", "rater_id", Name = "uq_rating_per_tx", IsUnique = true)]
public partial class user_rating
{
    [Key]
    public int id { get; set; }

    public int transaction_id { get; set; }

    public int rater_id { get; set; }

    public int rated_user_id { get; set; }

    public int score { get; set; }

    [StringLength(500)]
    public string? comment { get; set; }

    public DateTime created_at { get; set; }

    [ForeignKey("rated_user_id")]
    [InverseProperty("user_ratingrated_users")]
    public virtual user rated_user { get; set; } = null!;

    [ForeignKey("rater_id")]
    [InverseProperty("user_ratingraters")]
    public virtual user rater { get; set; } = null!;

    [ForeignKey("transaction_id")]
    [InverseProperty("user_ratings")]
    public virtual transaction transaction { get; set; } = null!;
}
