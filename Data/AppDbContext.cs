using System;
using System.Collections.Generic;
using Connect2Deal.Models;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<Listing> Listings { get; set; }

    public virtual DbSet<ListingImage> ListingImages { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRating> UserRatings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=connect2deal;Username=postgres;Password=1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("fk_categories_parent");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("conversations_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.LastMessage).WithMany(p => p.Conversations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_last_message");

            entity.HasOne(d => d.User1).WithMany(p => p.ConversationUser1s).HasConstraintName("fk_conv_user1");

            entity.HasOne(d => d.User2).WithMany(p => p.ConversationUser2s).HasConstraintName("fk_conv_user2");
        });

        modelBuilder.Entity<Listing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("listings_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.ExpiresAt).HasDefaultValueSql("(now() + '10 days'::interval)");
            entity.Property(e => e.Status).HasDefaultValueSql("'Active'::character varying");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.Category).WithMany(p => p.Listings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_listings_category");

            entity.HasOne(d => d.Location).WithMany(p => p.Listings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("listings_location_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Listings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_listings_user");
        });

        modelBuilder.Entity<ListingImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("listing_images_pkey");

            entity.Property(e => e.IsPrimary).HasDefaultValue(false); 
            entity.Property(e => e.SortOrder).HasDefaultValue(0);

            entity.HasOne(d => d.Listing).WithMany(p => p.ListingImages).HasConstraintName("listing_images_listing_id_fkey");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("locations_pkey");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("locations_parent_id_fkey");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("messages_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.MessageType).HasDefaultValueSql("'text'::character varying");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages).HasConstraintName("fk_msg_conversation");

            entity.HasOne(d => d.Listing).WithMany(p => p.Messages)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_msg_listing");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages).HasConstraintName("fk_msg_sender");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transactions_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Buyer).WithMany(p => p.TransactionBuyers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tx_buyer");

            entity.HasOne(d => d.Listing).WithOne(p => p.Transaction)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tx_listing");

            entity.HasOne(d => d.Seller).WithMany(p => p.TransactionSellers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tx_seller");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsBlocked).HasDefaultValue(false);
            entity.Property(e => e.IsEmailVerified).HasDefaultValue(false);
            entity.Property(e => e.Role).HasDefaultValueSql("'User'::character varying");
        });

        modelBuilder.Entity<UserRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_ratings_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.RatedUser).WithMany(p => p.UserRatingRatedUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rating_rated");

            entity.HasOne(d => d.Rater).WithMany(p => p.UserRatingRaters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rating_rater");

            entity.HasOne(d => d.Transaction).WithMany(p => p.UserRatings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rating_tx");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
