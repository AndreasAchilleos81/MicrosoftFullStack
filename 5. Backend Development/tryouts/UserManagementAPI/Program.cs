using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using Microsoft.AspNetCore.HttpLogging;

internal class Program
{
    static ConcurrentDictionary<int, User> users = new ConcurrentDictionary<int, User>();
    private static void Main(string[] args) 
    {
        for (int i = 0; i < 10; i++)
        {
            var user = User.GenerateUser();
            bool added = users.TryAdd(user.Id, user);
        }

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestPath |
                                    HttpLoggingFields.RequestBody |
                                    HttpLoggingFields.Duration |
                                    HttpLoggingFields.ResponseBody |
                                    HttpLoggingFields.ResponseStatusCode;
            options.RequestHeaders.Add("RQ-logged");
            options.ResponseHeaders.Add("RS-logged");

        });
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();
        app.UseHttpLogging();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection(); // TODO: setup https

        app.Use(async  (context, next) =>
        {
            try
            {
                await next.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine(context.Request.Path);
                Console.WriteLine(ex.ToString());
            }
        });

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

        app.MapPost("/users", ([FromBody] User user) =>
        {
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(user, new ValidationContext(user), validationResults, true))
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var issue in validationResults)
                {
                    stringBuilder.AppendLine(issue.ErrorMessage);
                }

                return Results.BadRequest(stringBuilder.ToString());
            }
            if (users.TryAdd(user.Id, user))
            {
                return Results.Ok($"Added user with Id: {user.Id}");
            }
            else
            {
                return Results.BadRequest($"Failed to add User with Id {user.Id}");
            }
        });


        app.MapPut("/users", ([FromBody] User user) =>
        {
            var newId = user.Id;
            var result = users.AddOrUpdate(newId, newId => user, (newId, existingUser) => user);
            return Results.Ok($"User with Id:{result.Id} was updated");
        });


        app.MapDelete("/users/{id:int}", (int id) =>
        {
            var removed = users.TryRemove(id, out User user);
            if (removed)
            {
                return Results.Ok($"Successfully removed user with Id: {user.Id}");
            }
            else
            {
                return Results.NotFound($"Removed user with Id: {id}");
            }
        });

        app.Run();
    }
}