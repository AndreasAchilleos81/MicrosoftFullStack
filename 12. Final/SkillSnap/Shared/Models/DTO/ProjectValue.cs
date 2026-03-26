namespace Shared.Models.DTO
{
    public class ProjectValue
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public List<PortfolioUser> PortfolioUsers { get; set; } = new();

        public Project ConverTo()
        {
            return new Project() 
            {
                Description = this.Description,
                Id = this.Id,
                ImageUrl = this.ImageUrl,
                PortfolioUsers = this.PortfolioUsers,
                Title = this.Title,
            };
        }
    }
}
