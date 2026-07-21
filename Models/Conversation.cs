using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Table("conversations")]
[Index("User1Id", "User2Id", Name = "idx_conversations_users")]
[Index("User1Id", "User2Id", Name = "uq_conversation", IsUnique = true)]
public partial class Conversation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user1_id")]
    public int User1Id { get; set; }

    [Column("user2_id")]
    public int User2Id { get; set; }

    [Column("last_message_id")]
    public int? LastMessageId { get; set; }

    [Column("last_message_at")]
    public DateTime? LastMessageAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("LastMessageId")]
    [InverseProperty("Conversations")]
    public virtual Message? LastMessage { get; set; }

    [InverseProperty("Conversation")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [ForeignKey("User1Id")]
    [InverseProperty("ConversationUser1s")]
    public virtual User User1 { get; set; } = null!;

    [ForeignKey("User2Id")]
    [InverseProperty("ConversationUser2s")]
    public virtual User User2 { get; set; } = null!;
}
