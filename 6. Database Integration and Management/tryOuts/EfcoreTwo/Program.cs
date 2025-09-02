using EfcoreTwo;
using Microsoft.EntityFrameworkCore;

HRDbContext dbContext = new HRDbContext();

//dbContext.Database.Migrate();

var entityTypes = dbContext.Model.GetEntityTypes();

var allEmployees = dbContext.Employees;

Console.WriteLine("ALL EMPLOYEES");
await allEmployees.ForEachAsync(e =>
{
	Console.WriteLine(e);
});



var employees = dbContext.Employees.Where(e => e.Department.Name == "HR");
Console.WriteLine("EMPLOYEES IN HR");
await employees.ForEachAsync(e =>
{
	Console.WriteLine(e);
});