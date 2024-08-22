using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;
using SampleAPI.Repositories;

namespace SampleAPI.Tests.Repositories
{
    public class OrderRepositoryTests
    {
        private readonly DbContextOptions<SampleApiDbContext> _options;

        public OrderRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<SampleApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetRecentOrders_ShouldReturnOrdersWithinLastDay()
        {
            // Arrange
            using (var context = new SampleApiDbContext(_options))
            {
                var repository = new OrderRepository(context);

                var recentOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    Name = "Recent Order",
                    Description = "Recent Description",
                    EntryDate = DateTime.UtcNow.AddHours(-12), // within last day
                    IsDeleted = false
                };

                var oldOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    Name = "Old Order",
                    Description = "Old Description",
                    EntryDate = DateTime.UtcNow.AddDays(-2), // older than 1 day
                    IsDeleted = false
                };

                context.Orders.AddRange(recentOrder, oldOrder);
                await context.SaveChangesAsync();

                // Act
                var result = await repository.GetRecentOrders(o => (DateTime.UtcNow - o.EntryDate) < TimeSpan.FromDays(1));

                // Assert
                result.Should().HaveCount(1); // Expecting only the recent order
                result.First().Should().BeEquivalentTo(recentOrder);
            }
        }

        [Fact]
        public async Task AddNewOrder_ShouldAddOrderToDatabase()
        {
            // Arrange
            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                Name = "New Order",
                Description = "New Description",
                EntryDate = DateTime.UtcNow
            };

            using (var context = new SampleApiDbContext(_options))
            {
                var repository = new OrderRepository(context);

                // Act
                await repository.AddNewOrder(newOrder);
                var result = await context.Orders.FindAsync(newOrder.Id);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(newOrder);
            }
        }

        [Fact]
        public async Task GetOrdersByDays_ShouldReturnOrdersExcludingWeekendsAndHolidays()
        {
            // Arrange
            var days = 7;
            using (var context = new SampleApiDbContext(_options))
            {
                var repository = new OrderRepository(context);

                var recentOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    Name = "Recent Order",
                    Description = "Recent Description",
                    EntryDate = DateTime.UtcNow.AddDays(-2)
                };

                var weekendOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    Name = "Weekend Order",
                    Description = "Weekend Description",
                    EntryDate = new DateTime(2024, 8, 18) // Sunday
                };

                var holidayOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    Name = "Holiday Order",
                    Description = "Holiday Description",
                    EntryDate = new DateTime(DateTime.UtcNow.Year, 8, 15) // Independence Day
                };

                context.Orders.AddRange(recentOrder, weekendOrder, holidayOrder);
                await context.SaveChangesAsync();

                // Act
                var result = await repository.GetOrdersByDays(days);

                // Assert
                result.Should().HaveCount(1);
                result.First().Should().BeEquivalentTo(recentOrder);
            }
        }
    }
}
