using HashTables;

ConcurrentHashTable<string, int> hashTable = new ConcurrentHashTable<string, int>();

hashTable.Add("Alice", 25);
hashTable.Add("Bob", 30);
hashTable.Add("Charlie", 35);

Console.WriteLine($"Alice's age: {hashTable.TryGetValue("Alice", out var AAge)} - Age: {AAge}");
Console.WriteLine($"Bob's age: {hashTable.TryGetValue("Bob", out var BAge)} - Age: {BAge}");
Console.WriteLine($"Charlie's age: {hashTable.TryGetValue("Charlie", out var CAge)} - Age: {CAge}");
