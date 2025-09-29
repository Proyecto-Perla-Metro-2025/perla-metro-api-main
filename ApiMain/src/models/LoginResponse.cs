using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMain.src.models
{
    public class LoginResponse
    {
        /// <summary>
        /// Token with the user's claims
        /// </summary>
        public required string Token { get; set; }
        /// <summary>
        ///  Date in wich the token expires
        /// </summary>
        public DateTime ExpiresAt { get; set; }
        /// <summary>
        /// User's information
        /// </summary>
        public required UserInfo User { get; set; }
    }
}