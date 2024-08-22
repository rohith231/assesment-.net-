using System.Linq.Expressions;
using SampleAPI.Entities;

namespace SampleAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetRecentOrders(Expression<Func<Order, bool>> predicate);
        Task AddNewOrder(Order order);
        Task<IEnumerable<Order>> GetOrdersByDays(int days);
    }
}
