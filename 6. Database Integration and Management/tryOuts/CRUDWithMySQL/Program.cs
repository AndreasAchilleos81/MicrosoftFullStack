using CRUDWithMySQL;
using CRUDWithMySQL.Models;
using Microsoft.EntityFrameworkCore;

var database = new ApplicationDbContext();

var product = new Product
{
	ArrivalDate = new DateOnly(year: DateTime.Today.Date.Year, month: DateTime.Today.Date.Month, day: DateTime.Today.Date.Day),
	Description = "Description",
	Name = "Name",
	Price = 200m
};

await database.Products.AddAsync(product);
await database.SaveChangesAsync();


var products = database.Products.ToList();

products.ForEach(p => Console.WriteLine(p.ToString()));

var getMeId = database.Products.FirstOrDefault();
var foundProduct = database.Products.Find(getMeId.Id);

foundProduct.Price = 999.99m;

database.Products.Update(foundProduct);
await database.SaveChangesAsync();

Console.WriteLine("Product after update");
var afterUpdateProduct = database.Products.Find(getMeId.Id);
Console.WriteLine(afterUpdateProduct);

Console.WriteLine("Removing updated product");
database.Products.Remove(afterUpdateProduct);
database.SaveChanges();

Console.WriteLine($"Products left in DB should be zero, they are: {database.Products.Count()}");



