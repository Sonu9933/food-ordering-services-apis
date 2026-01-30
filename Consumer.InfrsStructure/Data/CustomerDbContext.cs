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
            modelBuilder.Entity<Order>()
                .HasOne(e => e.OrderDetails)
                .WithMany()
                .HasForeignKey(e=>e.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(e => e.OrderDetails)
                .WithMany()
                .HasForeignKey(e => e.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(e => e.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(e => e.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuItems>()
                .HasOne(e => e.Restaurant)
                .WithMany()
                .HasForeignKey(e => e.RestaurantID)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
