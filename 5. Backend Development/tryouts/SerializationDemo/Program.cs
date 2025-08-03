using System.IO;
using System.Xml.Serialization;


public class Program
{
    public class Person
    {
        public string UserName { get; set; }
        public int UserAge { get; set; }
    }

    public static void Main(string[] args)
    {
        Person person = new Person
        {
            UserName = "JohnDoe",
            UserAge = 30
        };

        using FileStream fileStream = new FileStream(@"..\..\..\person.dat", FileMode.Create);
        using BinaryWriter writer = new BinaryWriter(fileStream);
        writer.Write(person.UserName);
        writer.Write(person.UserAge);

        Console.WriteLine("Person binary data serialization written to file successfully.");

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Person));
        using FileStream xmlFileStream = new FileStream(@"..\..\..\person.xml", FileMode.Create);
        xmlSerializer.Serialize(xmlFileStream, person);
        Console.WriteLine("Person XML serialization written to file successfully.");

        string jsonData = "{\"UserName\":\"" + person.UserName + "\",\"UserAge\":" + person.UserAge + "}";
        File.WriteAllText(@"..\..\..\person.json", jsonData);
        Console.WriteLine("Person JSON serialization written to file successfully.");
    }
}