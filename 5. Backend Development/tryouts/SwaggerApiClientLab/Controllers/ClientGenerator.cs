using System.Net.Http;
using System.Threading.Tasks;
using NSwag;
using NSwag.CodeGeneration.CSharp;

public class ClientGenerator
{
    public static async Task GenerateClient(string swaggerJsonPath, string outputPath)
    {
        using var httpClient = new HttpClient();
        var swaggerJson = await httpClient.GetStringAsync(swaggerJsonPath);
        var swaggerDocument = await OpenApiDocument.FromJsonAsync(swaggerJson);

        // Create a C# client generator settings
        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = "clientApi",
            CSharpGeneratorSettings =   { Namespace = "MyClient" }
        };

        // Create the client generator
        var generator = new CSharpClientGenerator(swaggerDocument, settings);

        // Generate the client code
        var code = generator.GenerateFile();

        // Write the generated code to the specified output path
        System.IO.File.WriteAllText(outputPath, code);
    }
}   