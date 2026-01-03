

Console.WriteLine("Fetching data...");

var task1 = FetchDataAsync();
var task2 = FetchDataAsync();
var task3 = FetchDataAsync();
var task4 =  FetchDataAsync();	

var results = await Task.WhenAll(task1, task2, task3, task4);

foreach (var result in results)
{
	Console.WriteLine(result);
}	


async Task<string> FetchDataAsync()
{
	Task.Delay(1000);
	return "HERE IS SOME DATA THAT i fetched";
}


