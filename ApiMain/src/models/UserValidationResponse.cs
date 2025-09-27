using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiMain.src.models
{
    public class UserValidationResponse
    {
        public bool IsValid { get; set; }
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Surename { get; set; }
        public required string Role { get; set; }
        public List<Claim> Claims { get; set; } = new();
        public required string ErrorMessage { get; set; }
    }
}