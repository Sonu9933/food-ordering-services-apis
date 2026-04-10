using Customer.Core.Enum;
using FoodOrderingServices.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FoodOrderingServices.Infrastructure.Data
{
    /// <summary>
    /// Provides database seeding logic for all core entities.
    /// </summary>
    /// <remarks>
    /// Each seed block is guarded by an existence check, making the seeder safe to run
    /// on every application start (idempotent). Insertion order respects FK constraints:
    /// Customers → Restaurants → MenuItems → Orders → OrderDetails.
    /// </remarks>
    public static class DataSeeder
    {
        // Fixed GUIDs keep seeds deterministic across runs

        // Customers
        private static readonly Guid _customer1Id = new("a1a1a1a1-0000-0000-0000-000000000001");
        private static readonly Guid _customer2Id = new("a2a2a2a2-0000-0000-0000-000000000002");
        private static readonly Guid _customer3Id = new("a3a3a3a3-0000-0000-0000-000000000003");

        // Restaurants
        private static readonly Guid _restaurant1Id = new("b1b1b1b1-0000-0000-0000-000000000001");
        private static readonly Guid _restaurant2Id = new("b2b2b2b2-0000-0000-0000-000000000002");
        private static readonly Guid _restaurant3Id = new("b3b3b3b3-0000-0000-0000-000000000003");

        // Menu Items — Restaurant 1 (Spice Garden)
        private static readonly Guid _menuItem1Id = new("c1c1c1c1-0000-0000-0000-000000000001");
        private static readonly Guid _menuItem2Id = new("c2c2c2c2-0000-0000-0000-000000000002");
        private static readonly Guid _menuItem3Id = new("c3c3c3c3-0000-0000-0000-000000000003");

        // Menu Items — Restaurant 2 (Burger Hub)
        private static readonly Guid _menuItem4Id = new("c4c4c4c4-0000-0000-0000-000000000004");
        private static readonly Guid _menuItem5Id = new("c5c5c5c5-0000-0000-0000-000000000005");
        private static readonly Guid _menuItem6Id = new("c6c6c6c6-0000-0000-0000-000000000006");

        // Menu Items — Restaurant 3 (Pizza Palace)
        private static readonly Guid _menuItem7Id = new("c7c7c7c7-0000-0000-0000-000000000007");
        private static readonly Guid _menuItem8Id = new("c8c8c8c8-0000-0000-0000-000000000008");
        private static readonly Guid _menuItem9Id = new("c9c9c9c9-0000-0000-0000-000000000009");

        // Orders
        private static readonly Guid _order1Id = new("d1d1d1d1-0000-0000-0000-000000000001");
        private static readonly Guid _order2Id = new("d2d2d2d2-0000-0000-0000-000000000002");
        private static readonly Guid _order3Id = new("d3d3d3d3-0000-0000-0000-000000000003");

        // Order Details
        private static readonly Guid _orderDetail1Id = new("e1e1e1e1-0000-0000-0000-000000000001");
        private static readonly Guid _orderDetail2Id = new("e2e2e2e2-0000-0000-0000-000000000002");
        private static readonly Guid _orderDetail3Id = new("e3e3e3e3-0000-0000-0000-000000000003");
        private static readonly Guid _orderDetail4Id = new("e4e4e4e4-0000-0000-0000-000000000004");
        private static readonly Guid _orderDetail5Id = new("e5e5e5e5-0000-0000-0000-000000000005");

        /// <summary>
        /// Seeds all entities into the database if they do not already exist.
        /// Call this after <see cref="WebApplication.Build"/> and before <see cref="WebApplication.Run"/>.
        /// </summary>
        /// <param name="serviceProvider">The application's <see cref="IServiceProvider"/>.</param>
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                await context.Database.MigrateAsync();

                await SeedCustomersAsync(context, logger);
                await SeedRestaurantsAsync(context, logger);
                await SeedMenuItemsAsync(context, logger);
                await SeedOrdersAsync(context, logger);
                await SeedOrderDetailsAsync(context, logger);

                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (DbUpdateException ex)
            {
                // Unwrap the full exception chain to surface the exact SQL error
                var inner = ex.InnerException;
                while (inner is not null)
                {
                    logger.LogError("Inner exception: {Message}", inner.Message);
                    inner = inner.InnerException;
                }

                logger.LogError(ex, "DbUpdateException while seeding. See inner exception messages above for the SQL error.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while seeding the database.");
                throw;
            }
        }

        // Customers

        private static async Task SeedCustomersAsync(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Customers.AnyAsync())
            {
                logger.LogInformation("Customers already seeded — skipping.");
                return;
            }

            var customers = new List<Core.Entity.Customer>
            {
                new()
                {
                    CustomerId   = _customer1Id,
                    CustomerName = "Alice Johnson",
                    Email        = "alice.johnson@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                    CreatedAt    = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt    = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                    LastLogin    = new DateTime(2024, 6, 1, 9, 30, 0, DateTimeKind.Utc)
                },
                new()
                {
                    CustomerId   = _customer2Id,
                    CustomerName = "Bob Martinez",
                    Email        = "bob.martinez@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@456"),
                    CreatedAt    = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt    = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                    LastLogin    = new DateTime(2024, 6, 5, 14, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    CustomerId   = _customer3Id,
                    CustomerName = "Carol Smith",
                    Email        = "carol.smith@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@789"),
                    CreatedAt    = new DateTime(2024, 3, 20, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt    = new DateTime(2024, 3, 20, 0, 0, 0, DateTimeKind.Utc),
                    LastLogin    = null
                }
            };

            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();

            // Clear the change tracker so previously saved entities do not interfere
            // with fixup logic when inserting the next batch of entities.
            context.ChangeTracker.Clear();

            logger.LogInformation("Seeded {Count} customers.", customers.Count);
        }

        // Restaurants

        private static async Task SeedRestaurantsAsync(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Restaurants.AnyAsync())
            {
                logger.LogInformation("Restaurants already seeded — skipping.");
                return;
            }

            var restaurants = new List<Restaurant>
            {
                new()
                {
                    RestaurantID   = _restaurant1Id,
                    RestaurantName = "Spice Garden",
                    Location       = "12 Curry Lane, Manchester, M1 2AB",
                    ContactNumber  = "0161123456",
                    CreatedAt      = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt      = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    RestaurantID   = _restaurant2Id,
                    RestaurantName = "Burger Hub",
                    Location       = "45 Grill Street, Birmingham, B1 3CD",
                    ContactNumber  = "0121987654",
                    CreatedAt      = new DateTime(2023, 7, 15, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt      = new DateTime(2023, 7, 15, 0, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    RestaurantID   = _restaurant3Id,
                    RestaurantName = "Pizza Palace",
                    Location       = "78 Dough Avenue, London, EC1 4EF",
                    ContactNumber  = "0207654321",
                    CreatedAt      = new DateTime(2023, 8, 10, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt      = new DateTime(2023, 8, 10, 0, 0, 0, DateTimeKind.Utc)
                }
            };

            await context.Restaurants.AddRangeAsync(restaurants);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            logger.LogInformation("Seeded {Count} restaurants.", restaurants.Count);
        }

        // Menu Items

        private static async Task SeedMenuItemsAsync(ApplicationDbContext context, ILogger logger)
        {
            if (await context.MenuItems.AnyAsync())
            {
                logger.LogInformation("MenuItems already seeded — skipping.");
                return;
            }

            var menuItems = new List<MenuItems>
            {
                // Spice Garden
                new()
                {
                    ItemID       = _menuItem1Id,
                    RestaurantID = _restaurant1Id,
                    ItemName     = "Chicken Tikka Masala",
                    Description  = "Tender chicken in a rich, spiced tomato-cream sauce.",
                    Price        = 1299,
                    Category     = "Mains"
                },
                new()
                {
                    ItemID       = _menuItem2Id,
                    RestaurantID = _restaurant1Id,
                    ItemName     = "Garlic Naan",
                    Description  = "Soft leavened bread brushed with garlic butter.",
                    Price        = 299,
                    Category     = "Breads"
                },
                new()
                {
                    ItemID       = _menuItem3Id,
                    RestaurantID = _restaurant1Id,
                    ItemName     = "Mango Lassi",
                    Description  = "Chilled yoghurt drink blended with fresh mango.",
                    Price        = 399,
                    Category     = "Drinks"
                },

                // Burger Hub
                new()
                {
                    ItemID       = _menuItem4Id,
                    RestaurantID = _restaurant2Id,
                    ItemName     = "Classic Cheeseburger",
                    Description  = "Beef patty with cheddar, lettuce, tomato and house sauce.",
                    Price        = 899,
                    Category     = "Mains"
                },
                new()
                {
                    ItemID       = _menuItem5Id,
                    RestaurantID = _restaurant2Id,
                    ItemName     = "Loaded Fries",
                    Description  = "Crispy fries topped with cheese sauce, bacon and jalapeños.",
                    Price        = 549,
                    Category     = "Sides"
                },
                new()
                {
                    ItemID       = _menuItem6Id,
                    RestaurantID = _restaurant2Id,
                    ItemName     = "Chocolate Milkshake",
                    Description  = "Thick, creamy chocolate shake.",
                    Price        = 449,
                    Category     = "Drinks"
                },

                // Pizza Palace
                new()
                {
                    ItemID       = _menuItem7Id,
                    RestaurantID = _restaurant3Id,
                    ItemName     = "Margherita Pizza",
                    Description  = "Classic tomato base, mozzarella and fresh basil.",
                    Price        = 1099,
                    Category     = "Mains"
                },
                new()
                {
                    ItemID       = _menuItem8Id,
                    RestaurantID = _restaurant3Id,
                    ItemName     = "Pepperoni Pizza",
                    Description  = "Generous pepperoni on a tomato and mozzarella base.",
                    Price        = 1199,
                    Category     = "Mains"
                },
                new()
                {
                    ItemID       = _menuItem9Id,
                    RestaurantID = _restaurant3Id,
                    ItemName     = "Tiramisu",
                    Description  = "Traditional Italian coffee dessert.",
                    Price        = 599,
                    Category     = "Desserts"
                }
            };

            await context.MenuItems.AddRangeAsync(menuItems);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            logger.LogInformation("Seeded {Count} menu items.", menuItems.Count);
        }

        // Orders

        private static async Task SeedOrdersAsync(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Orders.AnyAsync())
            {
                logger.LogInformation("Orders already seeded — skipping.");
                return;
            }

            var orders = new List<Order>
            {
                new()
                {
                    OrderID      = _order1Id,
                    CustomerID   = _customer1Id,
                    RestaurantID = _restaurant1Id,
                    Status       = OrderStatus.Delivered,
                    OrderDate    = new DateTime(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc),
                    TotalAmount  = 1598  // Chicken Tikka Masala x1 (1299) + Garlic Naan x1 (299)
                },
                new()
                {
                    OrderID      = _order2Id,
                    CustomerID   = _customer2Id,
                    RestaurantID = _restaurant2Id,
                    Status       = OrderStatus.Confirmed,
                    OrderDate    = new DateTime(2024, 6, 5, 18, 30, 0, DateTimeKind.Utc),
                    TotalAmount  = 1448  // Classic Cheeseburger x1 (899) + Loaded Fries x1 (549)
                },
                new()
                {
                    OrderID      = _order3Id,
                    CustomerID   = _customer3Id,
                    RestaurantID = _restaurant3Id,
                    Status       = OrderStatus.Pending,
                    OrderDate    = new DateTime(2024, 6, 10, 20, 0, 0, DateTimeKind.Utc),
                    TotalAmount  = 2198  // Margherita Pizza x2 (1099 × 2)
                }
            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            logger.LogInformation("Seeded {Count} orders.", orders.Count);
        }

        // Order Details

        private static async Task SeedOrderDetailsAsync(ApplicationDbContext context, ILogger logger)
        {
            if (await context.OrderDetails.AnyAsync())
            {
                logger.LogInformation("OrderDetails already seeded — skipping.");
                return;
            }

            var orderDetails = new List<OrderDetail>
            {
                // Order 1 — Spice Garden
                new()
                {
                    OrderDetailID = _orderDetail1Id,
                    OrderID       = _order1Id,
                    ItemID        = _menuItem1Id,  // Chicken Tikka Masala
                    Quantity      = 1,
                    UnitPrice     = 1299
                },
                new()
                {
                    OrderDetailID = _orderDetail2Id,
                    OrderID       = _order1Id,
                    ItemID        = _menuItem2Id,  // Garlic Naan
                    Quantity      = 1,
                    UnitPrice     = 299
                },

                // Order 2 — Burger Hub
                new()
                {
                    OrderDetailID = _orderDetail3Id,
                    OrderID       = _order2Id,
                    ItemID        = _menuItem4Id,  // Classic Cheeseburger
                    Quantity      = 1,
                    UnitPrice     = 899
                },
                new()
                {
                    OrderDetailID = _orderDetail4Id,
                    OrderID       = _order2Id,
                    ItemID        = _menuItem5Id,  // Loaded Fries
                    Quantity      = 1,
                    UnitPrice     = 549
                },

                // Order 3 — Pizza Palace
                new()
                {
                    OrderDetailID = _orderDetail5Id,
                    OrderID       = _order3Id,
                    ItemID        = _menuItem7Id,  // Margherita Pizza
                    Quantity      = 2,
                    UnitPrice     = 1099
                }
            };

            await context.OrderDetails.AddRangeAsync(orderDetails);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            logger.LogInformation("Seeded {Count} order detail lines.", orderDetails.Count);
        }
    }
}