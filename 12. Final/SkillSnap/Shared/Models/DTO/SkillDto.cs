using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models.DTO
{
    public class SkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Level { get; set; }

        public Skill ConvertTo()
        {
            return new Skill { Id = Id, Name = Name, Level = Level };
        }
    }
}
