using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMain.src.models
{
    public class LoginRequest
    {
        /// <summary>
        /// User's email
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// User's password
        /// </summary>
        public required string Password { get; set; }
    }
}