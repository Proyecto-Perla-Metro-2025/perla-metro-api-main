using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMain.src.models
{
    public class LoginResponse
    {
        public required string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public required UserInfo User { get; set; }
    }
}