using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Table("transactions")]
[Index("ListingId", Name = "uq_tx_listing", IsUnique = true)]
public partial class Transaction
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("listing_id")]
    public int ListingId { get; set; }

    [Column("seller_id")]
    public int SellerId { get; set; }

    [Column("buyer_id")]
    public int BuyerId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("BuyerId")]
    [InverseProperty("TransactionBuyers")]
    public virtual User Buyer { get; set; } = null!;

    [ForeignKey("ListingId")]
    [InverseProperty("Transaction")]
    public virtual Listing Listing { get; set; } = null!;

    [ForeignKey("SellerId")]
    [InverseProperty("TransactionSellers")]
    public virtual User Seller { get; set; } = null!;

    [InverseProperty("Transaction")]
    public virtual ICollection<UserRating> UserRatings { get; set; } = new List<UserRating>();
}
