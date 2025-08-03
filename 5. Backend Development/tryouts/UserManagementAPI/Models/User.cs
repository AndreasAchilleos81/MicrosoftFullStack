using System.ComponentModel.DataAnnotations;
using Bogus;

namespace UserManagementAPI.Models
{
    public class User 
    {
        private static Faker faker = new Faker();
        [Required]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public DateTime DateOfHire { get; set; }
        public DateTime? DateOfTermination { get; set; }
        public string? ReasonOfTermination { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public static User GenerateUser()
        {
            return new User
            {
                Id = faker.Random.Int(1, 1000),
                FirstName = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                Email = faker.Person.Email,
                Department = faker.Company.CompanyName(),
                Position = faker.Name.JobTitle(),
                DateOfHire = faker.Date.Past(3),
                IsActive = true
            };
        }
    }
}