
// await PerformLongOperationAsync();
// Console.WriteLine("Main method completed");

// await  Task.Run(() => PerformLongOperationAsync());

// Console.WriteLine("Main method completed");

    double result = Math.Sqrt(-2);

await Task.Run(async () => await PerformLongOperationAsync());


async Task PerformLongOperationAsync(){
    
    double result = Math.Sqrt(-2);

    Console.WriteLine("Long operation Started");
    await Task.Delay(1000);
    Console.WriteLine("Long operation completed");
}
