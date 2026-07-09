using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Index("listing_id", Name = "uq_tx_listing", IsUnique = true)]
public partial class transaction
{
    [Key]
    public int id { get; set; }

    public int listing_id { get; set; }

    public int seller_id { get; set; }

    public int buyer_id { get; set; }

    public DateTime created_at { get; set; }

    [ForeignKey("buyer_id")]
    [InverseProperty("transactionbuyers")]
    public virtual user buyer { get; set; } = null!;

    [ForeignKey("listing_id")]
    [InverseProperty("transaction")]
    public virtual listing listing { get; set; } = null!;

    [ForeignKey("seller_id")]
    [InverseProperty("transactionsellers")]
    public virtual user seller { get; set; } = null!;

    [InverseProperty("transaction")]
    public virtual ICollection<user_rating> user_ratings { get; set; } = new List<user_rating>();
}
