using crudWebApi.Models;
using System.Collections.Concurrent;



var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Welcome to the Simple Web API!");

app.MapGet("/items", () => ItemsCollection.itemsDictionary.Select(i => i.Value));

app.MapGet("/items/{id:int}", (int id) => 
{
	if (ItemsCollection.itemsDictionary.TryGetValue(id, out var item))
	{
		return Results.Ok(item);
	}
	return Results.NotFound();
});

app.MapPost("/items", (Item item) => 
{
	if (item == null || string.IsNullOrWhiteSpace(item.Name))
	{
		return Results.BadRequest("Item cannot be null and must have a name.");
	}
	if (ItemsCollection.itemsDictionary.TryAdd(item.Id, item))
	{
		return Results.Created($"/items/{item.Id}", item);
	}
	return Results.Conflict("An item with the same ID already exists.");
});

app.MapPut("/items/{id:int}", (int id, Item item) => 
{
	if (item == null || string.IsNullOrWhiteSpace(item.Name))
	{
		return Results.BadRequest("Item cannot be null and must have a name.");
	}
	if (ItemsCollection.itemsDictionary.ContainsKey(id))
	{
		ItemsCollection.itemsDictionary[id] = item;
		return Results.Ok(item);
	}
	return Results.NotFound();
});

app.MapDelete("/items/{id:int}", (int id) => 
{
	if (ItemsCollection.itemsDictionary.TryRemove(id, out var item))
	{
		return Results.Ok(item);
	}
	return Results.NotFound();
});	

app.Run();
