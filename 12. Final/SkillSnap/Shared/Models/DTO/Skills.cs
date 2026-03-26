namespace Shared.Models.DTO
{
    public class Skills
    {
        public List<SkillsValue> value { get; set; } = new();

        public string? executionMs { get; set; }
    }

    public class SkillsValue
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Level { get; set; }

    }
}
