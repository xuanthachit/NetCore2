using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    public class ProductsController : Controller
    {
        private readonly IDutchRepository _repository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IDutchRepository repository, ILogger<ProductsController> logger)
        {
            this._repository = repository;
            this._logger = logger;
        }

        [HttpGet]
        // JsonResult Get()
        public IEnumerable<Product> Get()
        {
            try
            {
                //return Json(this._repository.GetAllProducts());
                return this._repository.GetAllProducts();
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Failed to get all products: {ex}");
                //return Json("Bad request");
                return null;
            }
        }
    }
}