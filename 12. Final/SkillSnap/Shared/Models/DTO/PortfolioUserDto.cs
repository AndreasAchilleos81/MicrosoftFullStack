using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models.DTO
{
    public class PortfolioUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Bio { get; set; }
        public string ProfilePictureUrl { get; set; }

        public List<SkillDto> Skills { get; set; } = new(); 

        public PortfolioUser ConvertTo()
        {
            return new PortfolioUser
            {
                Id = this.Id,
                Name = this.Name,
                Bio = this.Bio,
                ProfilePictureUrl = this.ProfilePictureUrl,
                Skills = this.Skills.ConvertAll(s => s.ConvertTo())
            };  
        }
    }
}
