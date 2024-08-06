using BlogAPI.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    // DbSets represent the collections of the specified entities in the database
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }

    // Constructor to pass DbContextOptions to the base class
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Configures the schema needed for the Identity framework and other entities
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Ensures that the base class configurations are applied
        base.OnModelCreating(builder);

        // Configures ApplicationUser entity
        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.UserName)  // Ensures UserName is unique
            .IsUnique();

        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)  // Ensures Email is unique
            .IsUnique();

        // Configures BlogPost entity
        builder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.Id);  // Primary key
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);  // Title is required and has a max length of 200
            entity.Property(e => e.Content).IsRequired();  // Content is required
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");  // Default value for CreatedAt is the current date
            entity.HasOne(e => e.User)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete behavior
        });

        // Configures Comment entity
        builder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);  // Primary key
            entity.Property(e => e.Content).IsRequired();  // Content is required
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");  // Default value for CreatedAt is the current date
            entity.HasOne(e => e.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete behavior
            entity.HasOne(e => e.BlogPost)
                .WithMany(p => p.Comments)
                .HasForeignKey(e => e.BlogPostId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete behavior
            entity.HasOne(e => e.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(e => e.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete behavior
        });

        // Configures Like entity
        builder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id);  // Primary key
            entity.HasOne(e => e.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete behavior
            entity.HasOne(e => e.BlogPost)
                .WithMany(p => p.Likes)
                .HasForeignKey(e => e.BlogPostId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete behavior
            entity.HasOne(e => e.Comment)
                .WithMany(c => c.Likes)
                .HasForeignKey(e => e.CommentId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict delete behavior

            // Ensure unique likes
            entity.HasIndex(e => new { e.UserId, e.BlogPostId }).IsUnique();  // Unique constraint on UserId and BlogPostId
            entity.HasIndex(e => new { e.UserId, e.CommentId }).IsUnique();  // Unique constraint on UserId and CommentId
        });
    }
}