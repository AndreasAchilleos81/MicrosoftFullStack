using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogicTrack.Models
{
	public class Order
	{
		[Key]
		public int OrderId { get; set; }
		public string CustomerName { get; set; } = string.Empty;

		// Order contains many order lines
		public List<OrderItem> Items { get; set; } = new List<OrderItem>();

		public DateTime DatePlaced { get; set; }

		public void AddItem(OrderItem item) => Items.Add(item);
		public void RemoveItem(OrderItem item) => Items.Remove(item);

		public string GetOrderSummary()
		{
			var totalLines = Items.Count;
			var totalQuantity = Items.Sum(i => i.Quantity);
			return $"Order ID: {OrderId}, Customer: {CustomerName}, Lines: {totalLines}, TotalQty: {totalQuantity}, Date Placed: {DatePlaced}";
		}
	}
}