namespace Shared.Models.DTO
{
    public class Projects
    {
        public List<ProjectValue> value { get; set; } = new();

        public string executionMs { get; set; }
    }

    public class ProjectValue
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public List<PortfolioUser> PortfolioUsers { get; set; }
    }
}
