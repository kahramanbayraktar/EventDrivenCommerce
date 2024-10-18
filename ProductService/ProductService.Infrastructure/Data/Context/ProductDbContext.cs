using Bogus;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Data.Configurations;

namespace ProductService.Infrastructure.Data.Context
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Product> Products { get; set; }

        public static void SeedDatabase(ProductDbContext context)
        {
            if (context.Products.Count() > 0)
                return;

            //using var transaction = context.Database.BeginTransaction();

            try
            {
                var random = new Random();

                // 2. Generate Random Products
                var productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => f.Random.Decimal(1.00m, 1000.00m))
                .RuleFor(p => p.SKU, f => f.Commerce.Ean13())
                .RuleFor(p => p.Category, f => f.PickRandom(f.Commerce.Categories(10)))
                .RuleFor(p => p.ImageUrl, f => f.Image.LoremFlickrUrl())
                .RuleFor(p => p.CreatedAt, f => f.Date.Past(2));

                var products = productFaker.Generate(10000);

                // Add products to the database in batches
                context.Products.AddRange(products);
                context.SaveChanges();

                // Commit the transaction if everything succeeds
                //transaction.Commit();
            }
            catch (Exception)
            {
                // Rollback the transaction if any error occurs
                //transaction.Rollback();
                throw;
            }
        }
    }
}
