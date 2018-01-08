using DutchTreat.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    public class DutchRepository : IDutchRepository
    {
        private readonly DutchContext _context;
        private readonly ILogger<DutchRepository> _logger;

        public DutchRepository(DutchContext context, ILogger<DutchRepository> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        public void AddEntity(object order)
        {
            _context.Add(order);
        }

        public void AddOrder(Order newOrder)
        {
            foreach (var item in newOrder.Items)
            {
                item.Product = _context.Products.Find(item.Product.Id);
            }
            AddEntity(newOrder);
        }

        public IEnumerable<Order> GetAllOrders(bool includeItems)
        {
            if (includeItems)
            {
                return _context.Orders
                        .Include(x => x.Items)
                        .ThenInclude(i => i.Product)
                        .ToList(); 
            }
            else
            {
                return _context.Orders
                        .ToList();
            }
        }

        public IEnumerable<Order> GetAllOrdersByUser(string userName, bool includeItems)
        {
            if (includeItems)
            {
                return _context.Orders
                        .Where(x => x.User.UserName == userName)
                        .Include(x => x.Items)
                        .ThenInclude(i => i.Product)
                        .ToList();
            }
            else
            {
                return _context.Orders
                        .Where(x => x.User.UserName == userName)
                        .ToList();
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                _logger.LogInformation("GetAllProducts was called");
                var products = _context.Products
                    .OrderBy(x => x.Title)
                    .ToList();

                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fail to get all products {ex}");
                return null;
            }
        }

        public Order GetOrderById(int id)
        {
            return _context.Orders
                .Include(x => x.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.Id == id)
                .FirstOrDefault();
        }

        public Order GetOrderById(string name, int orderId)
        {
            return _context.Orders
                .Include(x => x.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.Id == orderId && o.User.UserName == name)
                .FirstOrDefault();
        }

        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            var products = _context.Products
                .Where(x => x.Category == category)
                .ToList();

            return products;
        }

        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
