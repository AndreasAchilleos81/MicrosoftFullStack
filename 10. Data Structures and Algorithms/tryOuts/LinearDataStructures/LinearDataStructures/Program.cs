string[] expressions = new string[] { "3 + 5", "10 - 2 * 3", "(4 + 6) / 2", "8 * (3 + 2) - 5" };
LinkedList<string> expressionList = new LinkedList<string>();
expressionList.AddLast("Result: 8");
expressionList.AddLast("Result: 5");
expressionList.AddLast("Result: 28");
expressionList.AddLast("Result: 4");
expressionList.AddLast("Result: 9");

Stack<string> expressionStack = new Stack<string>();
expressionStack.Push("5 + 3");
expressionStack.Push("10 - 2");
expressionStack.Push("7 * 4");
expressionStack.Push("20 / 5");
expressionStack.Push("3 ^ 2");

Queue<string> expressionQueue = new Queue<string>();
expressionQueue.Enqueue("Calculate: 15 + 5");
expressionQueue.Enqueue("Calculate: 12 - 3");
expressionQueue.Enqueue("Calculate: 9 * 2");
expressionQueue.Enqueue("Calculate: 16 / 4");
expressionQueue.Enqueue("Calculate: 2 ^ 3");

PrintExpressionQueue(expressionQueue);
Console.WriteLine($"Initial expressions:\n {expressionQueue.Dequeue()}");

// PrintExpressions(expressions);
//PrintExpressionList(expressionList);
// Console.WriteLine($"Last entry added{expressionStack.Pop()}");
PrintExpressionQueue(expressionQueue);

expressions[1] = "10 / 2";
expressionList.Remove("Result: 5");

Console.WriteLine("\nAfter modification:\n");

// PrintExpressions(expressions);
//PrintExpressionList(expressionList);
// PrintExpressionStack(expressionStack);

static void PrintExpressions(string[] expressions)
{
    foreach (var expr in expressions)
    {
        Console.WriteLine($"Expression: {expr} ");
    }
}

static void PrintExpressionList(LinkedList<string> expressionList)
{
    foreach (var result in expressionList)
    {
        Console.WriteLine(result);
    }
}

static void PrintExpressionStack(Stack<string> expressionStack)
{
    foreach (var expr in expressionStack)
    {
        Console.WriteLine($"Stack Expression: {expr} ");
    }
}

static void PrintExpressionQueue(Queue<string> expressionQueue)
{
    foreach (var expr in expressionQueue)
    {
        Console.WriteLine($"Queue Expression: {expr} ");
    }
}



