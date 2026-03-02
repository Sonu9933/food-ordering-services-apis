using Microsoft.EntityFrameworkCore;
using FoodOrderingServices.Core.Entity;

namespace FoodOrderingServices.Infrastructure.Data
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the food ordering application, providing access to
    /// customers, orders, order details, restaurants, and menu items.
    /// </summary>
    /// <remarks>This context defines entity sets for core business objects and configures entity
    /// relationships and indexes using the Fluent API. It should be registered with the application's dependency
    /// injection container to enable data access throughout the application.</remarks>
    /// <param name="options">The options used to configure the database context, including database provider, connection string, and other
    /// context-specific settings.</param>
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public virtual DbSet<Core.Entity.Customer> Customers { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        public virtual DbSet<Restaurant> Restaurants { get; set; }

        public virtual DbSet<MenuItems> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Index's
            ConfigureIndexes(modelBuilder);

            //Relationship's
            ConfigureRelationships(modelBuilder);
        }

        private static void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Core.Entity.Customer>()
                .HasIndex(c => c.Email)
                .IsUnique()
                .HasDatabaseName("ix_customer_email");

            modelBuilder.Entity<Restaurant>()
                .HasIndex(o => o.RestaurantID)
                .IsUnique()
                .HasDatabaseName("ix_restaurant_id");

            modelBuilder.Entity<MenuItems>()
                .HasIndex(o => o.ItemID)
                .IsUnique()
                .HasDatabaseName("ix_menu_items_id");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderID)
                .IsUnique()
                .HasDatabaseName("ix_order_id");

            modelBuilder.Entity<OrderDetail>()
                .HasIndex(o => o.OrderDetailID)
                .IsUnique()
                .HasDatabaseName("ix_order_detail_id");
        }

        private static void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Order -> Customer relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            // Order -> Restaurant relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .HasForeignKey(e => e.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderDetail -> Order relationship (One-to-Many)
            modelBuilder.Entity<OrderDetail>()
                .HasOne(e => e.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(e => e.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderDetail -> MenuItem relationship
            modelBuilder.Entity<OrderDetail>()
                .HasOne(e => e.MenuItem)
                .WithMany()
                .HasForeignKey(e => e.ItemID)
                .OnDelete(DeleteBehavior.Restrict);

            // MenuItem -> Restaurant relationship
            modelBuilder.Entity<MenuItems>()
                .HasOne(e => e.Restaurant)
                .WithMany()
                .HasForeignKey(e => e.RestaurantID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
