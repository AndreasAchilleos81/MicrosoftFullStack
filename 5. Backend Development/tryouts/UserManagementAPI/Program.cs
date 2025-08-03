using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;


ConcurrentDictionary<int, User> users = new ConcurrentDictionary<int, User>();

for (int i = 0; i < 10; i++)
{
    var user = User.GenerateUser();
    bool added = users.TryAdd(user.Id, user);
}


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection(); // TODO: setup https

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/", () => "I AM GET-ROOT");

app.MapGet("/users", () =>
{
    return Results.Ok(users);
});

app.MapGet("/users/{id:int}", (int id) =>
{
    User user = null;
    if (users.TryGetValue(id, out user))
    {
        return Results.Ok(user);
    }
    return Results.NotFound();
});

app.MapPost("/users", ([FromBody]User user) =>
{
    if (users.TryAdd(user.Id, user))
    {
        return Results.Ok($"Added user with Id: {user.Id}");
    }
    else
    {
        return Results.BadRequest($"Failed to add User with Id {user.Id}");
    }
});


app.MapPut("/users", ([FromBody]User user) =>
{
    var newId = user.Id;
    users.AddOrUpdate(newId, newId => user , (newId, existinUser) => user);  
});


app.MapDelete("/users/{id:int}", (int id)=>
{
    var removed = users.TryRemove(id, out User user);
    if (removed)
    {
        Results.Ok($"Successfully removed user with Id: {user.Id}");
    }
    else
    {
        Results.NotFound($"Removed user with Id: {id}");
    }
});

app.Run();


