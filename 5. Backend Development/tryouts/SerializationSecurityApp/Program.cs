Console.WriteLine("Serialization Security risks");

User user = new User("John Doe", "koks@dmail.c", "password123");

Console.WriteLine("Serialized User: " + user.Serialize());

var jsonString = "{\"Name\":\"John Doe\",\"Email\":\"koks@dmail.c\"}";

var user2 = user.Deserialize(jsonString);
user2.SetPassword("password123");
Console.WriteLine("Deserialized User: " + user2.Serialize() + "IS Password Valid: " + user2.ValidatePassword());


