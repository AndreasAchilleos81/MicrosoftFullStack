using System.Text.Json;
using System.Xml.Serialization;

public class Person
{
    public string UserName { get; set; }
    public int UserAge { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        var binaryStopWatch = System.Diagnostics.Stopwatch.StartNew();
        // Serialize to binary

        using var filestream = new System.IO.FileStream("person.dat", System.IO.FileMode.Open);
        using var binaryReader = new System.IO.BinaryReader(filestream);
        var deserializedPerson = new Person
        {
            UserName = binaryReader.ReadString(),
            UserAge = binaryReader.ReadInt32()
        };

        Console.WriteLine($"BinaryDeserialized time seconds {binaryStopWatch.Elapsed.TotalSeconds} Person: {deserializedPerson.UserName}, Age: {deserializedPerson.UserAge}");


        // Deserialize to XML
        var xmlStopwatch = System.Diagnostics.Stopwatch.StartNew();

        var xmlData = File.ReadAllText("person.xml");

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Person));
        using var reader = new StringReader(xmlData);
        var xmlDeserializedPerson = (Person)xmlSerializer.Deserialize(reader);
        Console.WriteLine($"XMLDeserialized time seconds {xmlStopwatch.Elapsed.TotalSeconds} Person: {xmlDeserializedPerson.UserName}, Age: {xmlDeserializedPerson.UserAge}");

        var jsonStopwatch = System.Diagnostics.Stopwatch.StartNew();
        // Serialize to JSON
        var jsonData = File.ReadAllText("person.json");
        Person jsonDeserializedPerson = JsonSerializer.Deserialize<Person>(jsonData);
        Console.WriteLine($"JSONDeserialized time seconds {jsonStopwatch.Elapsed.TotalSeconds} Person: {jsonDeserializedPerson.UserName}, Age: {jsonDeserializedPerson.UserAge}");
        
    }
}