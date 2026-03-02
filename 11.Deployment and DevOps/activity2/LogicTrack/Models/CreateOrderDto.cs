namespace LogicTrack.Models
{
	public partial class OrderController
	{
		// DTO for creating an order
		public class CreateOrderDto
		{
			public string CustomerName { get; set; } = string.Empty;
			public List<int> InventoryItemIds { get; set; } = new List<int>();
			public DateTime? DatePlaced { get; set; }
		}
	}
}
