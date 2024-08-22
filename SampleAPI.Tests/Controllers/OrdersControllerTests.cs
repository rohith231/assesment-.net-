using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SampleAPI.Controllers;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using System.Linq.Expressions;

namespace SampleAPI.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly OrdersController _controller;
        private readonly Mock<IOrderRepository> _mockOrderRepository;

        public OrdersControllerTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _controller = new OrdersController(_mockOrderRepository.Object);
        }

        [Fact]
        public async Task GetOrders_ShouldReturn200_WhenOrdersAreFound()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), Name = "Order1", Description = "Description1", EntryDate = DateTime.UtcNow.AddHours(-1) },
                new Order { Id = Guid.NewGuid(), Name = "Order2", Description = "Description2", EntryDate = DateTime.UtcNow.AddHours(-2) }
            };

            _mockOrderRepository.Setup(repo => repo.GetRecentOrders(It.IsAny<Expression<Func<Order, bool>>>()))
                                .ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedOrders = okResult.Value as List<Order>;
            returnedOrders.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetOrders_ShouldReturn404_WhenNoOrdersAreFound()
        {
            // Arrange
            _mockOrderRepository.Setup(repo => repo.GetRecentOrders(It.IsAny<Expression<Func<Order, bool>>>()))
                                .ReturnsAsync(new List<Order>());

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturn201_WhenOrderIsCreated()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                Name = "New Order",
                Description = "New Description"
            };

            _mockOrderRepository.Setup(repo => repo.AddNewOrder(It.IsAny<Order>()))
                                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateOrder(request);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturn400_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var result = await _controller.CreateOrder(new CreateOrderRequest());

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task GetOrdersByDays_ShouldReturn200_WhenOrdersAreFound()
        {
            // Arrange
            var days = 5;
            var orders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), Name = "Order1", Description = "Description1", EntryDate = DateTime.UtcNow.AddDays(-1) },
                new Order { Id = Guid.NewGuid(), Name = "Order2", Description = "Description2", EntryDate = DateTime.UtcNow.AddDays(-2) }
            };

            _mockOrderRepository.Setup(repo => repo.GetOrdersByDays(days))
                                .ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrdersByDays(days);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedOrders = okResult.Value as List<Order>;
            returnedOrders.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetOrdersByDays_ShouldReturn404_WhenNoOrdersAreFound()
        {
            // Arrange
            var days = 5;
            _mockOrderRepository.Setup(repo => repo.GetOrdersByDays(days))
                                .ReturnsAsync(new List<Order>());

            // Act
            var result = await _controller.GetOrdersByDays(days);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}
