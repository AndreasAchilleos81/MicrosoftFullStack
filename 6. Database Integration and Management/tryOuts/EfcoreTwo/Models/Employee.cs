namespace EfcoreTwo.Models
{
	public class Employee
	{
		public int Id { get; set; }
		
		public string FirstName { get; set; }
		
		public string LastName { get; set; }
		
		public string Email { get; set; }

		public decimal Salary { get; set; }

		public string Phone { get; set; }

		public int DepartmentId { get; set; }

		// Navigation Property
		public Department Department { get; set; }
	}
}
