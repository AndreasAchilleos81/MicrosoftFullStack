using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using WebApiFromConsole.Models;
using WebApiFromConsole.ModelsFactory;

namespace WebApiFromConsole.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProductsController : ControllerBase
    {
        private static ConcurrentDictionary<int, Product> _products { get; set; } = new ConcurrentDictionary<int, Product>();
        
        static ProductsController()
        {
            var products = new List<Product>();
            for (int i = 0; i < 10; i++)
            {
                var product = ProductFactory.GetProduct();
                var key = product.Id ?? 0;
                _products.TryAdd(key, ProductFactory.GetProduct());
            }
        }

        [HttpGet]
        public ActionResult<List<Product>> Get()
        {
            return Ok(_products);
        }

        [HttpGet("featured")]
        public Product GetFeaturedProduct()
        {
            return ProductFactory.GetSponsoredProduct();
        }

        [HttpPost]
        public ActionResult<bool> Post([FromBody] Product newProduct)
        {
            var key = newProduct.Id ?? 0;
            _products.TryAdd(key, newProduct);
            return true;
        }

        [HttpPut("{id}")]
        public ActionResult<string> Put([FromBody] Product updatedProduct)
        {
            if (updatedProduct == null)
            {
                return NotFound($"Product with ID {updatedProduct.Id} not found.");
            }

            _products.AddOrUpdate(updatedProduct.Id ?? 0, updatedProduct, (key, oldValue) => updatedProduct);
            return $"Updated Product ID {updatedProduct.Id} to {updatedProduct}";
        }

        [HttpDelete("{id}")]
        public ActionResult<string> Delete(int id)
        {

            if (!_products.TryRemove(id, out var removedProduct))
            {
                return NotFound($"Product with ID {id} not found.");
            }   

            return $"Deleted Product ID {id}";
        }
    }
}