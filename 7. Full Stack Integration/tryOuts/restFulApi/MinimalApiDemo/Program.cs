internal class Program
{
    static List<Counter> counters = new();

    private static void Main(string[] args)
    {
        counters.Add(
            new Counter
            {
                Id = 1,
                Name = "First",
                Count = 0,
            }
        );
        counters.Add(
            new Counter
            {
                Id = 2,
                Name = "Second",
                Count = 0,
            }
        );

        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        app.MapGet("/counters", () => counters);

        app.MapPost(
            "/counters",
            (Counter counter) =>
            {
                if (counters.Any(c => c.Name == counter.Name))
                {
                    return Results.Conflict($"Counter with Name {counter.Name} already exists.");
                }
                else
                {
                    counters.Add(counter);
                    return Results.Created($"/counters/{counter.Id}", counter);
                }
            }
        );

        app.MapPut(
            "/counters/increment/{id}",
            (int id) =>
            {
                var counter = counters.FirstOrDefault(c => c.Id == id);
                if (counter == null)
                {
                    return Results.NotFound();
                }

                counter.Count++;
                return Results.Ok(counter);
            }
        );

        app.MapDelete(
            "/counters/{id}",
            (int id) =>
            {
                var counter = counters.FirstOrDefault(c => c.Id == id);
                if (counter == null)
                {
                    return Results.NotFound();
                }

                counters.Remove(counter);
                return Results.NoContent();
            }
        );

        app.Run();
    }
}

internal class Counter
{
    public int Id { get; set; }

    public string Name { get; set; }
    public int Count { get; set; }
}
