namespace LogicTrack.Models
{
	public class OrderSummaryDto
	{
		public int OrderId { get; set; }
		public string CustomerName { get; set; } = string.Empty;
		public int ItemCount { get; set; }
		public DateTime DatePlaced { get; set; }
	}
}