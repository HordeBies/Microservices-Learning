﻿using Catalog.DataAccess.Repositories;
using Catalog.Entities;
using Catalog.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;
using System.Net;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository productRepository;
        private readonly ILogger<CatalogController> logger;

        public CatalogController(IProductRepository productRepository, ILogger<CatalogController> logger)
        {
            this.productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "GetProducts")]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var user = User;
            var products = await productRepository.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await productRepository.GetProduct(id);
            if (product == null)
            {
                logger.LogError($"Product with id: {id}, not found.");
                return NotFound();
            }
            return Ok(product);
        }

        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var products = await productRepository.GetProductsByCategory(category);
            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles = "admin,owner")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            await productRepository.CreateProduct(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [Authorize(Roles = "admin,owner")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            // TODO: Use a MassTransit to publish an event to RabbitMQ when a product is updated.
            return Ok(await productRepository.UpdateProduct(product));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [Authorize(Roles = "admin,owner")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            // TODO: Use a MassTransit to publish an event to RabbitMQ when a product is no longer available.
            return Ok(await productRepository.DeleteProduct(id));
        }
    }
}
