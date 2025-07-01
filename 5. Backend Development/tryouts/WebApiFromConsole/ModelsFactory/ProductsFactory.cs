using WebApiFromConsole.Models;
using Bogus;

namespace WebApiFromConsole.ModelsFactory
{
    public class ProductFactory
    {
        private static int _nextId = 1;

        public static Product GetProduct()
        {
            return new Faker<Product>()
                .RuleFor(p => p.Id, f => _nextId++)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Finance.Amount(1, 1000));
        }

        public static Product CreateProduct(string name, decimal price)
        {
            return new Product(_nextId++, name, price);
        }
        
        public static Product GetSponsoredProduct()
        {
            return GetProduct();
        }   
    }
}