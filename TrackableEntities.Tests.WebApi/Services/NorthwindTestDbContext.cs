using Microsoft.EntityFrameworkCore;
using TrackableEntities.EF.Core.Tests.Mocks;
using TrackableEntities.EF.Core.Tests.NorthwindModels;

namespace TrackableEntities.Tests.WebApi.Services
{
    public class NorthwindTestDbContext : DbContext
    {
        public NorthwindTestDbContext(DbContextOptions<NorthwindTestDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerSetting> CustomerSettings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Territory> Territories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductInfo>().HasKey(k => new { k.ProductInfoKey1, k.ProductInfoKey2 });
            modelBuilder.Entity<CustomerSetting>().Property(s => s.CustomerId).IsRequired();           
        }

        public void Initialize()
        {
            var model = new MockNorthwind();
            Categories.AddRange(model.Categories);
            Products.AddRange(model.Products);
            Customers.AddRange(model.Customers);
            Orders.AddRange(model.Orders);
            Territories.AddRange(model.Territories);
            Employees.AddRange(model.Employees);
            SaveChanges();
        }

        public void Clear()
        {
            RemoveRange(Categories);
            RemoveRange(Products);
            RemoveRange(Customers);
            RemoveRange(Orders);
            RemoveRange(Territories);
            RemoveRange(Employees);
            SaveChanges();
        }
    }
}
