using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Table("user_ratings")]
[Index("TransactionId", "RaterId", Name = "uq_rating_per_tx", IsUnique = true)]
public partial class UserRating
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("transaction_id")]
    public int TransactionId { get; set; }

    [Column("rater_id")]
    public int RaterId { get; set; }

    [Column("rated_user_id")]
    public int RatedUserId { get; set; }

    [Column("score")]
    public int Score { get; set; }

    [Column("comment")]
    [StringLength(500)]
    public string? Comment { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("RatedUserId")]
    [InverseProperty("UserRatingRatedUsers")]
    public virtual User RatedUser { get; set; } = null!;

    [ForeignKey("RaterId")]
    [InverseProperty("UserRatingRaters")]
    public virtual User Rater { get; set; } = null!;

    [ForeignKey("TransactionId")]
    [InverseProperty("UserRatings")]
    public virtual Transaction Transaction { get; set; } = null!;
}
