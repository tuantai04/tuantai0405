using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TechBlog.Models;

public partial class TechBlogContext : DbContext
{
    public TechBlogContext()
    {
    }

    public TechBlogContext(DbContextOptions<TechBlogContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=TechBlog;Trusted_Connection=True;TrustServerCertificate=True; Connection Timeout=3600");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.CategoryDesc).HasMaxLength(1000);
            entity.Property(e => e.CategoryName).HasMaxLength(1000);
            entity.Property(e => e.CategoryParrentId).HasDefaultValueSql("((0))");
            entity.Property(e => e.CategorySlug).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.IsDelete).HasDefaultValueSql("((1))");
            entity.Property(e => e.Levels).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(e => e.CommentContent).HasMaxLength(2000);
            entity.Property(e => e.CommentDate).HasColumnType("datetime");
            entity.Property(e => e.CommentLevels).HasDefaultValueSql("((1))");
            entity.Property(e => e.CommentParrentId).HasDefaultValueSql("((0))");
            entity.Property(e => e.FullName).HasMaxLength(50);

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_Comments_Post");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("Contact");

            entity.Property(e => e.ContactDesc).HasMaxLength(50);
            entity.Property(e => e.ContactDetail).HasColumnType("ntext");
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Gmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("Post");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");
            entity.Property(e => e.IsHot).HasDefaultValueSql("((0))");
            entity.Property(e => e.LikeNumber).HasDefaultValueSql("((0))");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PostDetail).HasColumnType("ntext");
            entity.Property(e => e.PostImage).HasMaxLength(2000);
            entity.Property(e => e.PostName).HasMaxLength(2000);
            entity.Property(e => e.PostSlug).HasMaxLength(2000);
            entity.Property(e => e.PostStatus).HasDefaultValueSql("((1))");
            entity.Property(e => e.PostTitle).HasMaxLength(2000);
            entity.Property(e => e.ViewNumber).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Category).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Post_Category");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PostCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Post_Users");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PostModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Post_Users1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.Avatar).HasColumnType("ntext");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
