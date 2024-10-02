using Application.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class LibraryDbContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Penalty> Penalties { get; set; }

    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book to Publisher (Many-to-One)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.Cascade);

            // Book to BookCopy (One-to-Many)
            modelBuilder.Entity<Book>()
                .HasMany(b => b.BookCopies)
                .WithOne(bc => bc.Book)
                .HasForeignKey(bc => bc.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Book to Genre (Many-to-Many)
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookGenre",
                    j => j
                        .HasOne<Genre>()
                        .WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Book>()
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade));

            // BookCopy to Transaction (One-to-Many)
            modelBuilder.Entity<BookCopy>()
                .HasMany(bc => bc.Transactions)
                .WithOne(t => t.BookCopy)
                .HasForeignKey(t => t.BookCopyId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser to Transaction (One-to-Many)
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Transactions)
                .WithOne(t => t.ApplicationUser)
                .HasForeignKey(t => t.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser to Penalty (One-to-Many)
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Penalties)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transaction to Penalty (One-to-Many)
            modelBuilder.Entity<Transaction>()
                .HasMany(t => t.Penalties)
                .WithOne(p => p.Transaction)
                .HasForeignKey(p => p.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Define property constraints and configurations
            modelBuilder.Entity<Book>()
                .Property(b => b.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Address)
                .HasMaxLength(200);

            modelBuilder.Entity<BookCopy>()
                .Property(bc => bc.Location)
                .HasMaxLength(50);


            //index
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique(); 

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Name); 

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.PublisherId);

            
            modelBuilder.Entity<BookCopy>()
                .HasIndex(bc => bc.BookId)
                .IsUnique(); 

            
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique(); 

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique(); 

            
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.Id)
                .IsUnique(); 

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.CheckoutDate);

            
            modelBuilder.Entity<Penalty>()
                .HasIndex(p => new { p.Amount, p.IssuedDate })
                .IsUnique();

            
        }

    }
}

