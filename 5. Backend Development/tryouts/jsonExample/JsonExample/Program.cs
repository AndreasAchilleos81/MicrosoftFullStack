using Newtonsoft.Json;

public class Program
{
    public static void Main(string[] args)
    {
        var jsonPerson = "{\"Name\":\"Alice\",\"Age\":30}";

        // Deserialize JSON to Person object
        Person person = JsonConvert.DeserializeObject<Person>(jsonPerson)!;
        Console.WriteLine(person.ToString());

        Console.WriteLine("And Back");
        // Serialize Person object back to JSON
        string jsonOutput = JsonConvert.SerializeObject(person, Formatting.Indented);
        Console.WriteLine(jsonOutput);
    }
}


class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public override string ToString()
    {
        return $"{Name}, {Age} years old";
    }
}