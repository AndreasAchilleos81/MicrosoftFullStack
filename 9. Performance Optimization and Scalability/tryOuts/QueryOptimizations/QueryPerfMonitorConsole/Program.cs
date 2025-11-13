using QueryPerfMonitorConsole;

const string connectionString = "Server=localhost;Database=AdventureWorks2022;User Id=sa;Password=Password1;TrustServerCertificate=True;";


const string personDataQuery = @"
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

const string postalCodeCoWorkersQuery = @"
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


QueryPerformanceMonitor monitor = new QueryPerformanceMonitor(connectionString);

var personDataMonitorResults = await monitor.MonitorQuery(personDataQuery);
var postalCodeMonitorResults = await monitor.MonitorQuery(postalCodeCoWorkersQuery);

Console.WriteLine("Person personal data query Performance Report:");	
await monitor.DisaplayPerformanceReport(personDataMonitorResults);

Console.WriteLine("\n\n\n\n");

Console.WriteLine("Postal code personal data query Performance Report:");
await monitor.DisaplayPerformanceReport(postalCodeMonitorResults);