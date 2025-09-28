using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMain.src.DTOs
{
    public class CreateUserDto
    {
        /// <summary>
        /// User's name
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// User's lastname
        /// </summary>
        public required string Surename { get; set; }
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