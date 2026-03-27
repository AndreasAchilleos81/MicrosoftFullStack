namespace Shared.Models.DTO
{
    public class Skills
    {
        public List<SkillDto> value { get; set; } = new();

        public string? executionMs { get; set; }
    }
}
