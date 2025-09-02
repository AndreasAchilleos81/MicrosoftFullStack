using Bogus;
using EfcoreTwo.Models;

namespace HrDbContext.DatabaseContext
{
	public static class Seeder
	{
		public static void SeedIfNecessary(this HrDatabase hrDatabase, bool addMoreData = false)
		{	if (addMoreData)
			{
			}
			else
			{
				if (hrDatabase.Departments.Count() == 0)
				{
					hrDatabase.AddRange(GenerateDeparments());
					hrDatabase.SaveChanges();
				}

				if (hrDatabase.Employees.Count() == 0)
				{
					hrDatabase.AddRange(GenerateEmployees(hrDatabase));
				}

			}

			hrDatabase.SaveChanges();
		}

		private static IEnumerable<Employee> GenerateEmployees(HrDatabase hrDatabase)
		{
			var ids = hrDatabase.Departments.ToList().Select(d => d.Id);
			var list = new List<Employee>();

			foreach (var id in ids)
			{
				for (int i = 0; i < 3; i++) {
					list.Add(GetEmployee(id));
				}
			}
			return list;
		}

		private static Employee GetEmployee(int departmentId)
		{

			return new Faker<Employee>().RuleFor(e => e.FirstName, f => f.Name.FirstName())
									    .RuleFor(e => e.FirstName, f => f.Name.LastName())
										.RuleFor(e => e.DepartmentId, f => departmentId);
		}

		private static IEnumerable<Department> GenerateDeparments()
		{
			var list = new List<Department>();
			for (int i = 0; i < 5; i++)
			{
				list.Add(GetDepartment());
			}
			return list;
		}

		private static Department GetDepartment()
		{
			return new Faker<Department>()
							.RuleFor(d => d.Name, f => f.Name.FirstName())
							.RuleFor(d => d.Description, f => f.Lorem.Sentence());
		}
	}
}
