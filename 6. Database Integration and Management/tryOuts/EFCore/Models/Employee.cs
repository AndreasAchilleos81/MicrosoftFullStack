public class Employee
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime HireDate { get; set; }
    
    // Foreign Key
    public int DepartmentId { get; set; }
    
    // Navigation Property (Many employees belong to one department)
    public Department Department { get; set; }
}