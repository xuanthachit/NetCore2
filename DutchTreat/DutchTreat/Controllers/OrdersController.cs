using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : Controller
    {
        private readonly IDutchRepository repository;
        private readonly ILogger<OrdersController> logger;
        private readonly IMapper mapper;
        private readonly UserManager<StoreUsers> userManager;

        public OrdersController(IDutchRepository repository, 
            ILogger<OrdersController> logger,
            IMapper mapper,
            UserManager<StoreUsers> userManager)
        {
            this.repository = repository;
            this.logger = logger;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Get(bool includeItems = true)
        {
            try
            {
                //var orders = mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(
                //        repository.GetAllOrders(includeItems)
                //    );


                // Get All Order by User
                var userName = User.Identity.Name;
                var orders = mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(
                        repository.GetAllOrdersByUser(userName, includeItems)
                    );

                return Ok(orders);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get all orders: {ex}");
                return BadRequest("Bad request");
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var order = this.repository.GetOrderById(id);
                if (order != null)
                    return Ok(mapper.Map<Order, OrderViewModel>(order));
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get all orders: {ex}");
                return BadRequest("Bad request");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newOrder = mapper.Map<OrderViewModel, Order>(model);

                    if(newOrder.OrderDate == DateTime.MinValue)
                    {
                        newOrder.OrderDate = DateTime.Now;
                    }

                    var currentUser = await this.userManager.FindByNameAsync(User.Identity.Name);
                    newOrder.User = currentUser;

                    repository.AddOrder(newOrder);
                    if (repository.SaveAll())
                    {
                        return Created($"/api/orders/{newOrder.Id}", mapper.Map<Order, OrderViewModel>(newOrder));
                    } 
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to save new order {ex}");
                return BadRequest("Bad request");
            }

            return BadRequest("Failed to save new Order");
        }
    }
}