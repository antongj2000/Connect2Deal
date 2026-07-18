using System;
using System.Collections.Generic;

namespace Connect2Deal.Models;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Description { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ProfileImage { get; set; }

    public string Role { get; set; } = null!;

    public bool IsEmailVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsBlocked { get; set; }

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();

    public virtual ICollection<Transaction> TransactionBuyers { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionSellers { get; set; } = new List<Transaction>();

    public virtual ICollection<UserRating> UserRatingRatedUsers { get; set; } = new List<UserRating>();

    public virtual ICollection<UserRating> UserRatingRaters { get; set; } = new List<UserRating>();
}
