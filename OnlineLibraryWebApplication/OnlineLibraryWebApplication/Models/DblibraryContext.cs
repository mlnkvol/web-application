using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineLibraryWebApplication.Models;

public partial class DblibraryContext : DbContext
{
    public DblibraryContext()
    {
    }

    public DblibraryContext(DbContextOptions<DblibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Possession> Possessions { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(e => e.Author1)
                .HasMaxLength(100)
                .HasColumnName("Author");

            entity.HasMany(d => d.Books).WithMany(p => p.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "AuthorsBook",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AuthorsBooks_Books"),
                    l => l.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AuthorsBooks_Authors"),
                    j =>
                    {
                        j.HasKey("AuthorId", "BookId").HasName("PK_AuthorsBooks_1");
                        j.ToTable("AuthorsBooks");
                    });
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(e => e.Image).HasColumnType("image");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_Publisher");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Categories_1");

            entity.Property(e => e.CategoryName).HasMaxLength(50);

            entity.HasMany(d => d.Books).WithMany(p => p.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoriesBook",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CategoriesBooks_Books"),
                    l => l.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CategoriesBooks_Categories"),
                    j =>
                    {
                        j.HasKey("CategoryId", "BookId").HasName("PK_CategoriesBooks_1");
                        j.ToTable("CategoriesBooks");
                    });
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Genres_1");

            entity.Property(e => e.GenreName).HasMaxLength(50);

            entity.HasMany(d => d.Books).WithMany(p => p.Genres)
                .UsingEntity<Dictionary<string, object>>(
                    "GenresBook",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_GenresBooks_Books"),
                    l => l.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_GenresBooks_Genres"),
                    j =>
                    {
                        j.HasKey("GenreId", "BookId").HasName("PK_GenresBooks_1");
                        j.ToTable("GenresBooks");
                    });
        });

        modelBuilder.Entity<Possession>(entity =>
        {
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Possessions)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Possessions_Books");

            entity.HasOne(d => d.User).WithMany(p => p.Possessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Possessions_Users");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Publishers_1");

            entity.Property(e => e.PublisherName).HasMaxLength(100);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasOne(d => d.Book).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Books");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
