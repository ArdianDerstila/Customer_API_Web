using CustomerImageApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerImageApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerImage> CustomerImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Company).HasMaxLength(200);


                entity.HasIndex(e => new { e.FirstName, e.LastName });
                entity.HasIndex(e => e.Email);
            });


            modelBuilder.Entity<CustomerImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageData).IsRequired();
                entity.Property(e => e.FileName).HasMaxLength(100);
                entity.Property(e => e.ContentType).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);


                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Images)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);


                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.UploadedAt);
            });


            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Phone = "555-0123",
                    Company = "Tech Corp",
                    CreatedAt = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                },
                new Customer
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    Phone = "555-0124",
                    Company = "Design Studio",
                    CreatedAt = new DateTime(2024, 1, 1, 11, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 11, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}