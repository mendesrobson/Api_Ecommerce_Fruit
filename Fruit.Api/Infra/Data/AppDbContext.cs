using Fruit.Api.Domain.Category;
using Fruit.Api.Domain.Customer;
using Fruit.Api.Domain.Product;
using Microsoft.EntityFrameworkCore;

namespace Fruit.Api.Infra.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
