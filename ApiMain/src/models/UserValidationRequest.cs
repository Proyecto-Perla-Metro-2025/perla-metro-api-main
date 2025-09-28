using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMain.src.models
{
    public class UserValidationRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}