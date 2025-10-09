var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

var app = builder.Build();
app.UseHttpsRedirection();

app.UseCors(policy =>
{
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
    policy.AllowAnyOrigin();
});

app.MapGet(
    "/api/productlist",
    () =>
    {
        return new[]
        {
            new
            {
                Id = 1,
                Name = "Laptop",
                Price = 1200.50,
                Stock = 25,
            },
            new
            {
                Id = 2,
                Name = "Headphones",
                Price = 50.00,
                Stock = 100,
            },
        };
    }
);

app.Run();
