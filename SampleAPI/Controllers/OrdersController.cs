using Microsoft.AspNetCore.Mvc;
using SampleAPI.Entities;
using SampleAPI.Repositories;

namespace SampleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("recent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            try
            {
                var now = DateTime.UtcNow;
                var recentOrders = await _orderRepository.GetRecentOrders(o => !o.IsDeleted && (now - o.EntryDate) < TimeSpan.FromDays(1));

                if (recentOrders == null || !recentOrders.Any())
                {
                    return NotFound("No recent orders found.");
                }

                var sortedOrders = recentOrders.OrderByDescending(o => o.EntryDate).ToList();

                return Ok(sortedOrders);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                EntryDate = DateTime.UtcNow,
                Name = request.Name,
                Description = request.Description,
                IsInvoiced = true,
                IsDeleted = false
            };

            try
            {
                await _orderRepository.AddNewOrder(newOrder);
                return CreatedAtAction(nameof(GetOrders), new { id = newOrder.Id }, newOrder);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("by-days")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Order>>> GetOrdersByDays(int days)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByDays(days);

                if (orders == null || !orders.Any())
                {
                    return NotFound("No orders found in the specified date range.");
                }

                return Ok(orders);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
