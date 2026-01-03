using System.Text.RegularExpressions;
using BinaryUserSearch;
using BinaryUserSearch.Types;
using Newtonsoft.Json;

const string randomUsersURL = "https://randomuser.me/api/?results=10";

HttpClient httpClient = new HttpClient();
HttpResponseMessage response = await httpClient.GetAsync(randomUsersURL);
Users users = null;

if (response.IsSuccessStatusCode)
{
	var resp = await response.Content.ReadAsStringAsync();
	resp = Regex.Unescape(resp);
	users = JsonConvert.DeserializeObject<Users>(resp);
	users.PrintUsers(Order.Desc);
}

else
{
	Console.WriteLine($"Error: {response.StatusCode}");
}

BinarySearch biSearch = new BinarySearch();
Console.WriteLine($"Searching for ... {users.results[2].ToString()}");
var found = biSearch.Search(users, users.results[2].name.first);

if (found != null)
{
	Console.WriteLine("User found:");
	Console.WriteLine(found);
}
else
{
	Console.WriteLine("User not found:");
}



