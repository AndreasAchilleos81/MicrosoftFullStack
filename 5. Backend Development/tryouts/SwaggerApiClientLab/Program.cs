using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

List<Blog> blogs = new List<Blog>
{
    new Blog { Id = 1, Title = "First Blog Post" },
    new Blog { Id = 2, Title = "Second Blog Post" }
};

const string swaggerUrl = "http://localhost:5000/swagger/v1/swagger.json";

var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
var clientCodePath = Path.GetFullPath(Path.Combine( path, @"..\..\..\", "GeneratedClient.cs"));

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000");
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseStaticFiles(); 
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));
}


app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.MapGet("/blogs", () => blogs)
   .Produces<List<Blog>>(StatusCodes.Status200OK, "application/json")
   .WithName("GetBlogs")
   .WithOpenApi(op =>
   {
       op.Summary     = "Get all blogs";
       op.Description = "Retrieves a list of all blog posts.";
       return op;
   });

app.MapGet("/blogs/{id}", Results<Ok<Blog>, NotFound> ([FromRoute] int id) =>
{ 
    var blog = blogs.FirstOrDefault(b => b.Id == id);
    if (blog is null)
    {
        return TypedResults.NotFound();
    }
    else
    {
        return TypedResults.Ok(blog);
    }
}).WithName("GetBlogById")
    .Produces<Blog>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);   


app.MapPost("/blogs", ([FromBody] Blog blog) =>
{
    blog.Id = blogs.Count + 1; // Simple ID assignment
    blogs.Add(blog);
    return Results.Created($"/blogs/{blog.Id}", blog);
})
.Produces<Blog>(StatusCodes.Status201Created);

await app.RunAsync();

class Blog
{
    public int Id { get; set; }
    public string Title { get; set; }
}