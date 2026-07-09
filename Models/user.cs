using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Index("email", Name = "uq_users_email", IsUnique = true)]
[Index("username", Name = "uq_users_username", IsUnique = true)]
public partial class user
{
    [Key]
    public int id { get; set; }

    [StringLength(50)]
    public string first_name { get; set; } = null!;

    [StringLength(50)]
    public string last_name { get; set; } = null!;

    [StringLength(50)]
    public string username { get; set; } = null!;

    [StringLength(100)]
    public string email { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    [StringLength(500)]
    public string? description { get; set; }

    [StringLength(20)]
    public string? phone_number { get; set; }

    [StringLength(255)]
    public string? profile_image { get; set; }

    [StringLength(20)]
    public string role { get; set; } = null!;

    public bool is_email_verified { get; set; }

    public DateTime created_at { get; set; }

    public bool is_blocked { get; set; }

    [InverseProperty("user")]
    public virtual ICollection<listing> listings { get; set; } = new List<listing>();

    [InverseProperty("buyer")]
    public virtual ICollection<transaction> transactionbuyers { get; set; } = new List<transaction>();

    [InverseProperty("seller")]
    public virtual ICollection<transaction> transactionsellers { get; set; } = new List<transaction>();

    [InverseProperty("rated_user")]
    public virtual ICollection<user_rating> user_ratingrated_users { get; set; } = new List<user_rating>();

    [InverseProperty("rater")]
    public virtual ICollection<user_rating> user_ratingraters { get; set; } = new List<user_rating>();
}
