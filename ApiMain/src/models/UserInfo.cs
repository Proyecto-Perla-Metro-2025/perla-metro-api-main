using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMain.src.models
{
    public class UserInfo
    {
        /// <summary>
        /// User's Id
        /// </summary>
        public required string Id { get; set; }
        /// <summary>
        /// User's email
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// User's role
        /// </summary>
        public required string Role { get; set; }
        /// <summary>
        /// User's fullname (name + surename)
        /// </summary>
        public required string FullName { get; set; }
        
    }
}