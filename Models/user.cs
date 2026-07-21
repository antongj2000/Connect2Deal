using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Table("users")]
[Index("Email", Name = "uq_users_email", IsUnique = true)]
[Index("Username", Name = "uq_users_username", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("first_name")]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [Column("username")]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("password_hash")]
    public string PasswordHash { get; set; } = null!;

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    [Column("phone_number")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Column("profile_image")]
    [StringLength(255)]
    public string? ProfileImage { get; set; }

    [Column("role")]
    [StringLength(20)]
    public string Role { get; set; } = null!;

    [Column("is_email_verified")]
    public bool IsEmailVerified { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("is_blocked")]
    public bool IsBlocked { get; set; }

    [InverseProperty("User1")]
    public virtual ICollection<Conversation> ConversationUser1s { get; set; } = new List<Conversation>();

    [InverseProperty("User2")]
    public virtual ICollection<Conversation> ConversationUser2s { get; set; } = new List<Conversation>();

    [InverseProperty("User")]
    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();

    [InverseProperty("Sender")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [InverseProperty("Buyer")]
    public virtual ICollection<Transaction> TransactionBuyers { get; set; } = new List<Transaction>();

    [InverseProperty("Seller")]
    public virtual ICollection<Transaction> TransactionSellers { get; set; } = new List<Transaction>();

    [InverseProperty("RatedUser")]
    public virtual ICollection<UserRating> UserRatingRatedUsers { get; set; } = new List<UserRating>();

    [InverseProperty("Rater")]
    public virtual ICollection<UserRating> UserRatingRaters { get; set; } = new List<UserRating>();
}
