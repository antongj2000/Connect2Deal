using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Table("messages")]
[Index("ConversationId", Name = "idx_messages_conversation")]
[Index("ReadAt", Name = "idx_messages_read")]
public partial class Message
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("conversation_id")]
    public int ConversationId { get; set; }

    [Column("sender_id")]
    public int SenderId { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [Column("message_type")]
    [StringLength(20)]
    public string MessageType { get; set; } = null!;

    [Column("listing_id")]
    public int? ListingId { get; set; }

    [Column("delivered_at")]
    public DateTime? DeliveredAt { get; set; }

    [Column("read_at")]
    public DateTime? ReadAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("ConversationId")]
    [InverseProperty("Messages")]
    public virtual Conversation Conversation { get; set; } = null!;

    [InverseProperty("LastMessage")]
    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    [ForeignKey("ListingId")]
    [InverseProperty("Messages")]
    public virtual Listing? Listing { get; set; }

    [ForeignKey("SenderId")]
    [InverseProperty("Messages")]
    public virtual User Sender { get; set; } = null!;
}
