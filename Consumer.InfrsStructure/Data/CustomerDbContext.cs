using Microsoft.EntityFrameworkCore;
using Customer.Core.Entity;

namespace ConsumerEnpoints.Data
{
    public class CustomerDbContext(DbContextOptions<CustomerDbContext> options) : DbContext(options)
    {
        public virtual DbSet<Customer.Core.Entity.Customer> Customers { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        public virtual DbSet<Restaurant> Restaurants { get; set; }

        public virtual DbSet<MenuItems> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Index's
            modelBuilder.Entity<Customer.Core.Entity.Customer>()
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

            //Relationship
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
