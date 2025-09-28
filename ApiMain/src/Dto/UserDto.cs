using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMain.src.Dto
{
    public class UserDto
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string SureName { get; set; }
        public required string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}