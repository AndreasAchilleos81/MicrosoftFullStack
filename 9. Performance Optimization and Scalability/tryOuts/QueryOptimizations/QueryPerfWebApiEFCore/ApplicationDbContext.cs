
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace QueryPerfWebApiEFCore
{
	public class ApplicationDbContext : DbContext
	{
		private readonly string _connectionString;

		private const string personDataQuery = @"
			USE[AdventureWorks2022]
			SELECT TOP 2000 p.BusinessEntityID AS Id, p.FirstName AS First_Name, p.MiddleName AS InBetween_Name, p.LastName AS Final_Name, p.NameStyle AS Name_Styl, e.EmailAddress AS EMAIL, pp.PhoneNumber, pnt.Name AS TypeOfPhoneNo, a.City, a.PostalCode
			FROM 
			Person.Person p
			JOIN Person.EmailAddress e ON p.BusinessEntityID = e.BusinessEntityID
			JOIN Person.PersonPhone pp ON p.BusinessEntityID = pp.BusinessEntityID
			JOIN Person.PhoneNumberType pnt ON pp.PhoneNumberTypeID = pnt.PhoneNumberTypeID
			JOIN Person.BusinessEntityAddress bea ON bea.BusinessEntityID = p.BusinessEntityID
			JOIN Person.Address a ON bea.AddressID = a.AddressID
			Order BY a.PostalCode;
			";

		private const string postalCodeCoWorkersQuery = @"
			USE[AdventureWorks2022]
			SELECT TOP 10  a.PostalCode, Count(*) AS Co_Workers 
			FROM 
			Person.Person p
			JOIN Person.EmailAddress e ON p.BusinessEntityID = e.BusinessEntityID
			JOIN Person.PersonPhone pp ON p.BusinessEntityID = pp.BusinessEntityID
			JOIN Person.PhoneNumberType pnt ON pp.PhoneNumberTypeID = pnt.PhoneNumberTypeID
			JOIN Person.BusinessEntityAddress bea ON bea.BusinessEntityID = p.BusinessEntityID
			JOIN Person.Address a ON bea.AddressID = a.AddressID
			Group BY a.PostalCode
			Order By a.PostalCode ASC;
			";

		public ApplicationDbContext(IConfiguration configuration)
		{
			_connectionString = configuration["DefaultConnection"]!;


		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder
				.UseSqlServer(_connectionString)
			   .LogTo(Console.WriteLine, LogLevel.Information)
			   .EnableSensitiveDataLogging()
			   .EnableDetailedErrors();
		}

		public async Task<int> ExecutePersonDetails() 
		{
			return await Database.ExecuteSqlRawAsync(personDataQuery);
		}

		public async Task<int> ExecuteWorkersPerPostCode() 
		{
			return await Database.ExecuteSqlRawAsync(postalCodeCoWorkersQuery);
		}

	}
}
