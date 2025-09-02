namespace CRUDWithMySQL.Models
{
	public class Product
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public DateOnly ArrivalDate { get; set; }

		public Decimal Price { get; set; }

		public override string ToString()
		{
			return $"{Name} {Description} {ArrivalDate} {Price}";
		}
	}
}
