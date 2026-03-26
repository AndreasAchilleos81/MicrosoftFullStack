namespace Shared.Models.DTO
{
    public class Projects
    {
        public List<ProjectValue> value { get; set; } = new();

        public string? executionMs { get; set; }
    }
}
