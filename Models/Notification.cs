using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Models;

[Table("notifications")]
public partial class Notification
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("type")]
    [StringLength(30)]
    public string Type { get; set; } = null!;

    [Column("message")]
    [StringLength(255)]
    public string Message { get; set; } = null!;

    [Column("is_read")]
    public bool IsRead { get; set; }

    [Column("related_id")]
    public int? RelatedId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User User { get; set; } = null!;
}
