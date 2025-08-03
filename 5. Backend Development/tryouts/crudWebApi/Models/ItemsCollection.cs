using System.Collections.Concurrent;

namespace crudWebApi.Models
{
	public static class ItemsCollection
	{
		public static ConcurrentDictionary<int, Item> itemsDictionary = new ConcurrentDictionary<int, Item>();

		static ItemsCollection()
		{
				Enumerable.Range(1, 10).ToList().ForEach(i => 
				{
					itemsDictionary.TryAdd(i, new Item { Id = i, Name = $"Item {i}" });
				});	
		}
	}
}
