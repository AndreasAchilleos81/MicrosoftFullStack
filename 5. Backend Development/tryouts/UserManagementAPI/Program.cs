using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using Microsoft.AspNetCore.HttpLogging;
using System.Text.Json;

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
                                    HttpLoggingFields.ResponseStatusCode|
                                    HttpLoggingFields.RequestBody |
                                    HttpLoggingFields.Duration |
                                    HttpLoggingFields.ResponseBody |
                                    HttpLoggingFields.ResponseStatusCode;
            options.RequestHeaders.Add("RQ-logged");
            options.ResponseHeaders.Add("RS-logged");

        });

        builder.Services.AddOpenApi();
        builder.Services.AddSingleton<Configuration>();
        var app = builder.Build();
        app.UseHttpLogging();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection(); // TODO: setup https

        // Exception handling MW
        app.Use(async  (context, next) =>
        {
            try
            {
                await next.Invoke();
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var errorResponse = new
                {
                    error = "internal server error",
                    path = context.Request.Path,
                    details = ex.Message
                };
                var json = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(json);

                Console.WriteLine($"Exception was caught at {context.Request.Path}");
                Console.WriteLine(ex.ToString());
            }
        });

        // Authorization MW
        app.Use(async (context, next) =>
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(token) ||
                isValidToken(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                var unAuth = new
                { 
                    error = "User is unauthorized",
                    path = context.Request.Path,
                    details = "Provide authorized token for access"
                };

                await context.Response.WriteAsJsonAsync(unAuth);
                return;
            }

            await next.Invoke();
        });

        // Logging MW
        app.Use(async (context, next) =>
        {
            var method = context.Request.Method;
            var path = context.Request.Path;

            await next.Invoke();

            var statusCode = context.Response.StatusCode;

            Console.WriteLine($"Method: {method}, Path: {path}, Status Code: {statusCode}");
        });


        bool isValidToken(string token)
        {
            return app.Configuration["token"] != token; 
        }

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