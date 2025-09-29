using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiMain.src.models
{
    public class UserValidationResponse
    {
        /// <summary>
        /// User's state
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// User's id
        /// </summary>
        public required string Id { get; set; }
        /// <summary>
        /// User's email
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// User's Name
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// User's surename
        /// </summary>
        public required string Surename { get; set; }
        /// <summary>
        /// User's role
        /// </summary>
        public required string Role { get; set; }
        /// <summary>
        /// User's claims
        /// </summary>
        public List<Claim> Claims { get; set; } = new();
        /// <summary>
        /// Possible error on the request
        /// </summary>
        public required string ErrorMessage { get; set; }
    }
}