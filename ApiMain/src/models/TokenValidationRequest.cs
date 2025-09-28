using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMain.src.models
{
    public class TokenValidationRequest
    {
        public required string Token { get; set; }
    }
}