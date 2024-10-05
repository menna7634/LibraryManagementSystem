using Application.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class LibraryDbContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryDbContext()
        { }
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Penalty> Penalties { get; set; }

		public DbSet<ContactForm> ContactForms { get; set; }



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
            //Book
            modelBuilder.Entity<Book>()
                .Property(b => b.Name)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Book>()
               .Property(b => b.ISBN)
               .HasMaxLength(13)
               .IsRequired();

            modelBuilder.Entity<Book>()
             .Property(b => b.Date)
             .IsRequired();

            modelBuilder.Entity<Book>()
                .Property(b => b.Author)
                .IsRequired();

            modelBuilder.Entity<Book>()
            .Property(b => b.Description)
            .HasMaxLength(1000).IsRequired(false);

            // Publisher 
            modelBuilder.Entity<Publisher>()
                .Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Publisher>()
                .Property(p => p.Location)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Publisher>()
                .Property(p => p.PhoneNumber)
                .HasMaxLength(15)
                .IsRequired(false);


            // Genre 
            modelBuilder.Entity<Genre>()
                .Property(g => g.Name)
                .HasMaxLength(50)
                .IsRequired();

            // BookCopy 
            modelBuilder.Entity<BookCopy>()
                .Property(bc => bc.Location)
                .HasMaxLength(50)
                .IsRequired(false);

            modelBuilder.Entity<BookCopy>()
                .Property(bc => bc.Available)
                .IsRequired();

            // ApplicationUser 

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Address)
                .HasMaxLength(200);

            modelBuilder.Entity<ApplicationUser>()
               .Property(u => u.DateOfBirth)
               .IsRequired();

            // Transaction 
            modelBuilder.Entity<Transaction>()
                .Property(t => t.CheckoutDate)
                .IsRequired();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.DueDate)
                .IsRequired();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.ReturnDate)
                .IsRequired();

            // Penalty 
            modelBuilder.Entity<Penalty>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            modelBuilder.Entity<Penalty>()
                .Property(p => p.IssuedDate)
                .IsRequired();

            modelBuilder.Entity<Penalty>()
                .Property(p => p.IsPaid)
                .IsRequired();

            modelBuilder.Entity<Penalty>()
                .Property(p => p.PaidAt)
                .IsRequired(false);

            //index
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique(); 

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Name); 

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.PublisherId);

            
            modelBuilder.Entity<BookCopy>()
                .HasIndex(bc => bc.Id)
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
                .HasIndex(p => new { p.Amount, p.IssuedDate });

            
        }

    }
}

