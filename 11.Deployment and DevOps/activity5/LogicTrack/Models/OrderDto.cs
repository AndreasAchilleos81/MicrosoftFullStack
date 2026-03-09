namespace LogicTrack.Models
{
	public class OrderItemDto
	{
		public int InventoryItemId { get; set; }
		public string InventoryItemName { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public decimal? UnitPrice { get; set; }
	}

	public class OrderDto
	{
		public int OrderId { get; set; }
		public string CustomerName { get; set; } = string.Empty;
		public DateTime DatePlaced { get; set; }
		public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
	}
}