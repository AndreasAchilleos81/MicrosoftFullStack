using System.Reflection;
using MyClient;
using System.Linq;

 var httpClient = new HttpClient();
clientApi clientApi = new clientApi("http://localhost:5000", httpClient);

var user = await clientApi.UserAsync(1);
Console.WriteLine($"User ID: {user.Id}, Name: {user.Name}, Email: {user.Email} ");

await clientApi.BlogsAsync(new Blog { Id = 4, Title = "Newest Blog" });

var blogs = (await clientApi.GetBlogsAsync()).ToList();

blogs.ForEach(b =>
{
    Console.WriteLine($"Blog ID: {b.Id}, Title: {b.Title}");
});

var blog3 = await clientApi.GetBlogByIdAsync(1);
Console.WriteLine($"Blog ID: {blog3.Id}, Title: {blog3.Title}");




var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
const string swaggerUrl = "http://localhost:5000/swagger/v1/swagger.json";
var clientCodePath = Path.GetFullPath(Path.Combine( path, @"..\..\..\", "GeneratedClient.cs"));

await ClientGenerator.GenerateClient(swaggerUrl, clientCodePath);


// class Blog
// {
//     public int Id { get; set; }
//     public string Title { get; set; }
// }