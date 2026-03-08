using System.ComponentModel.DataAnnotations;

namespace LogicTrack.Models
{
	public class InventoryItem : IEquatable<InventoryItem>
	{
		[Key]
		public int ItemId { get; set; }

		public string Name { get; set; } = string.Empty;

		// Stock on hand
		public int Quantity { get; set; }

		public string Location { get; set; } = string.Empty;

		// Navigation: many order lines can reference this inventory item
		public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

		public void DisplayInfo()
		{
			Console.WriteLine($"Item: {Name} | Quantity: {Quantity} | Location: {Location}");
		}

		public bool Equals(InventoryItem? other) => other?.ItemId == ItemId;
		public override bool Equals(object obj) => Equals(obj as InventoryItem);
		public override int GetHashCode() => ItemId.GetHashCode();

		public OrderItem ConvertTo()
		{
			return new OrderItem
			{
				InventoryItemId = this.ItemId,
				InventoryItem = this,
				Quantity = 1, // default quantity for new order item
				UnitPrice = null // price can be set later based on business logic
			};

		}
	}
}