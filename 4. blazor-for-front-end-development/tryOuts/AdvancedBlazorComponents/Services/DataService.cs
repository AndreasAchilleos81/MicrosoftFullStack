public class DataService
{
    public List<Product> GetProductsAsync()
    {
        // Simulate a delay for data fetching
        return new List<Product> {
            new Product { Name = "Product 1", Price = 10.99m, Description = "Description for Product 1", ImageUrl = "https://via.placeholder.com/150", Stock = 5 },
            new Product { Name = "Product 2", Price = 20.99m, Description = "Description for Product 2", ImageUrl = "https://via.placeholder.com/150", Stock = 3 },
            new Product { Name = "Product 3", Price = 30.99m, Description = "Description for Product 3", ImageUrl = "https://via.placeholder.com/150", Stock = 0 },
        };
    }
}


public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; }
    }