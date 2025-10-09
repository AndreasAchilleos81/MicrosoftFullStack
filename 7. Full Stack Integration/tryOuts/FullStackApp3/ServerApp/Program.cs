using Microsoft.Extensions.Caching.Memory; // Add for in-memory caching
using ServerApp.Constants;
using ServerApp.Services;

// Create a WebApplication builder using command-line arguments.
var builder = WebApplication.CreateBuilder(args);

// Register ProductService as a singleton service as recommended by Copilot.
builder.Services.AddSingleton<ProductService>();

// Register CORS services to allow cross-origin requests.
builder.Services.AddCors();

// Register in-memory caching service.
builder.Services.AddMemoryCache();

// Build the WebApplication instance.
var app = builder.Build();

// Redirect HTTP requests to HTTPS for security.
app.UseHttpsRedirection();

// Configure CORS policy to allow any header, method, and origin.
app.UseCors(policy =>
{
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
    policy.AllowAnyOrigin();
});

// Map a GET endpoint for "/api/productlist" that returns a cached list of products.
app.MapGet(
    "/api/productlist",
    (IMemoryCache cache) =>
    {
        // Try to get products from cache
        if (!cache.TryGetValue(CacheKeys.ProductList, out object? products))
        {
            // If not in cache, create the product list
            products = ProductService.GetProducts();

            // Set cache options (e.g., cache for 5 minutes)
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(
                TimeSpan.FromMinutes(5)
            );

            // Save data in cache
            cache.Set(CacheKeys.ProductList, products, cacheEntryOptions);
        }

        // Return cached products
        return products;
    }
);

// Start the application and begin listening for requests.
app.Run();

/*
Copilot Contribution:
GitHub Copilot suggested the use of IMemoryCache for caching, provided the logic for storing and retrieving
the product list from cache, and recommended cache expiration settings. Copilot also helped with comments
explaining the caching strategy to */
