using ApiCatalogue.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogue.Data
{
    public class AppCatalogueDBContext : DbContext
    {
        public AppCatalogueDBContext(DbContextOptions<AppCatalogueDBContext> options) : base(options) { }

        // Mapping classes
        public DbSet<Category>? Categorys { get; set; }
        public DbSet<Product>? Products { get; set; }

        // Using fluent api for configuration and replacement of ef core conversion in domain classes
        protected override void OnModelCreating(ModelBuilder mb)
        {
            // category
            mb.Entity<Category>().HasKey(c => c.Id);

            mb.Entity<Category>().Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            mb.Entity<Category>().Property(c => c.Description)
                .HasMaxLength(150)
                .IsRequired();

            // Product
            mb.Entity<Product>().HasKey(p => p.Id);

            mb.Entity<Product>().Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            mb.Entity<Product>().Property(c => c.Description)
                .HasMaxLength(150)
                .IsRequired();

            mb.Entity<Product>().Property(c => c.Price)
                .HasPrecision(14, 2);

            // relation one to many
            mb.Entity<Product>()
                .HasOne<Category>(p => p.Category)
                .WithMany(p => p.Products)
                .HasForeignKey(c => c.Id);

        }
    }
}
