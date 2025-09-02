namespace EfcoreTwo.Models
{
	public class Employee
	{
		public int Id { get; set; }

		public string FirstName { get; set; }

		public int DepartmentId { get; set; }

		// Navigation Property
		public Department Department { get; set; }
	}
}