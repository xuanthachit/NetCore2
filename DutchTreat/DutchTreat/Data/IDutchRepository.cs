using System.Collections.Generic;
using DutchTreat.Data.Entities;

namespace DutchTreat.Data
{
    public interface IDutchRepository
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetProductsByCategory(string category);
        IEnumerable<Order> GetAllOrders(bool includeItems);
        Order GetOrderById(int id);
        void AddEntity(object order);
        void AddOrder(Order newOrder);
        bool SaveAll();
        IEnumerable<Order> GetAllOrdersByUser(string userName, bool includeItems);
        Order GetOrderById(string name, int orderId);
    }
}