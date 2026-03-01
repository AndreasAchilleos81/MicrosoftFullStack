using System.ComponentModel.DataAnnotations;

namespace LogicTrack.Models
{
	public class OrderItem
	{
		[Key]
		public int OrderItemId { get; set; }

		// FK to Order
		public int OrderId { get; set; }
		public Order Order { get; set; } = null!;

		// FK to InventoryItem (the product)
		public int InventoryItemId { get; set; }
		public InventoryItem InventoryItem { get; set; } = null!;

		// Ordered quantity for this line
		public int Quantity { get; set; }

		// Optional: price at time of order
		public decimal? UnitPrice { get; set; }
	}
}