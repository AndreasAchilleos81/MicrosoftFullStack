

using BinarySearch;

var productIds = Enumerable.Range(1, 1000000000).ToArray();


//var productId = Search.BinarySearch(productIds, 1000000001);
int productId; 
Console.WriteLine((productId = Search.BinarySearch(productIds, 1000000001)) == -1 ? "Product not found" : $"Id of Product {productId}\nProduct {productIds[productId]}");


Console.WriteLine(Search.BinarySearch(productIds, 1000000001) is var id && id != -1 ? $"Id of Product {id}\nProduct {productIds[id]}" : "Product not found");
