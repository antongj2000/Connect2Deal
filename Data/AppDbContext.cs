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

    public virtual DbSet<category> categories { get; set; }

    public virtual DbSet<listing> listings { get; set; }

    public virtual DbSet<transaction> transactions { get; set; }

    public virtual DbSet<user> users { get; set; }

    public virtual DbSet<user_rating> user_ratings { get; set; }

  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<category>(entity =>
        {
            entity.HasKey(e => e.id).HasName("categories_pkey");

            entity.HasOne(d => d.parent).WithMany(p => p.Inverseparent).HasConstraintName("fk_categories_parent");
        });

        modelBuilder.Entity<listing>(entity =>
        {
            entity.HasKey(e => e.id).HasName("listings_pkey");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.expires_at).HasDefaultValueSql("(now() + '10 days'::interval)");
            entity.Property(e => e.status).HasDefaultValueSql("'Active'::character varying");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");
            entity.Property(e => e.view_count).HasDefaultValue(0);

            entity.HasOne(d => d.category).WithMany(p => p.listings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_listings_category");

            entity.HasOne(d => d.user).WithMany(p => p.listings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_listings_user");
        });

        modelBuilder.Entity<transaction>(entity =>
        {
            entity.HasKey(e => e.id).HasName("transactions_pkey");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.buyer).WithMany(p => p.transactionbuyers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tx_buyer");

            entity.HasOne(d => d.listing).WithOne(p => p.transaction)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tx_listing");

            entity.HasOne(d => d.seller).WithMany(p => p.transactionsellers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tx_seller");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("users_pkey");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_blocked).HasDefaultValue(false);
            entity.Property(e => e.is_email_verified).HasDefaultValue(false);
            entity.Property(e => e.role).HasDefaultValueSql("'User'::character varying");
        });

        modelBuilder.Entity<user_rating>(entity =>
        {
            entity.HasKey(e => e.id).HasName("user_ratings_pkey");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.rated_user).WithMany(p => p.user_ratingrated_users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rating_rated");

            entity.HasOne(d => d.rater).WithMany(p => p.user_ratingraters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rating_rater");

            entity.HasOne(d => d.transaction).WithMany(p => p.user_ratings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rating_tx");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
