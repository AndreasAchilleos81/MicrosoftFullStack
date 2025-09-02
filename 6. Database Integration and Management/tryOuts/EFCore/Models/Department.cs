public class Department
{public int DepartmentId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    // Navigation Property (One department has many employees)
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}