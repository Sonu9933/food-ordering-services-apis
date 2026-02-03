using Customer.Infrastructure.Data;
using Customer.Core.Contracts.Repositories;
using Customer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Caching.Distributed;

namespace Customer.UnitTests.Repositories
{
    internal class CustomerRepositaryTest
    {
        private ICustomerRepositary customerRepositary;
        private Mock<IDistributedCache> _mockCache;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var data = new List<Core.Entity.Customer>()
            {
                new Core.Entity.Customer()
                {
                    CreatedAt = DateTime.UtcNow,
                    CustomerId  = Guid.NewGuid(),
                    CustomerName = "Test",
                    Email = "test@gmail.com",
                    LastLogin = DateTime.UtcNow,
                    PasswordHash = "password",
                    UpdatedAt = DateTime.UtcNow
                }
            }.AsQueryable();
            var mockConsumer = new Mock<DbSet<Core.Entity.Customer>>();

            mockConsumer.As<IQueryable<Core.Entity.Customer>>().Setup(m => m.Provider).Returns(data.Provider);
            mockConsumer.As<IQueryable<Core.Entity.Customer >>().Setup(m => m.Expression).Returns(data.Expression);
            mockConsumer.As<IQueryable<Core.Entity.Customer >>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockConsumer.As<IQueryable<Core.Entity.Customer >>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var customerDbContext = new Mock<CustomerDbContext>(options);
            customerDbContext.Setup(m => m.Customers).Returns(mockConsumer.Object);

            _mockCache = new Mock<IDistributedCache>();

            customerRepositary = new CustomerRepositary(customerDbContext.Object, _mockCache.Object);
        }

        [Test]
        public void LoginCustomerAsync_Returns_Customer()
        {
            var result = customerRepositary.LoginCustomerAsync("test@gmail.com", "password");
            Assert.IsNotNull(result);
        }
    }
}
