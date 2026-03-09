namespace LogicTrack.Models
{
	public partial class OrderController
	{
		// DTO for creating an order
		public class CreateOrderDto
		{
			public string CustomerName { get; set; } = string.Empty;

			// Accept a list of item/quantity pairs so the client can specify quantities per line
			public List<OrderLineRequest> InventoryItems { get; set; } = new List<OrderLineRequest>();

			public DateTime? DatePlaced { get; set; }
		}

		public class OrderLineRequest
		{
			public int ItemId { get; set; }
			public int Quantity { get; set; } = 1;
		}
	}
}
