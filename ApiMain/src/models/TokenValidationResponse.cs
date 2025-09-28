using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiMain.src.models
{
    public class TokenValidationResponse
    {
        public bool IsValid { get; set; }
        public List<Claim> Claims { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}