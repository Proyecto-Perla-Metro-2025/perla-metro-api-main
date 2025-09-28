using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMain.src.models
{
    public class UserInfo
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required string FullName { get; set; }
    }
}