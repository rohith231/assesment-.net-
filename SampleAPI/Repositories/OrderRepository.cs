using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;

namespace SampleAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SampleApiDbContext _context;

        public OrderRepository(SampleApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetRecentOrders(Expression<Func<Order, bool>> predicate)
        {
            return await _context.Orders.Where(predicate).ToListAsync();
        }

        public async Task AddNewOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDays(int days)
        {
            var now = DateTime.UtcNow.Date;
            var startDate = now.AddDays(-days);

            var orders = await _context.Orders
                .Where(o => o.EntryDate >= startDate)
                .ToListAsync();

            // Filter out weekends
            orders = orders.Where(o => !IsWeekend(o.EntryDate) && !IsHoliday(o.EntryDate)).ToList();

            return orders;
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private bool IsHoliday(DateTime date)
        {
            return date.Month == 8 && date.Day == 15; // Independence Day
        }
    }
}
