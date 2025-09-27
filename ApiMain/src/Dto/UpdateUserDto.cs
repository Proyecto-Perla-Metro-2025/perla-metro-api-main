using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMain.src.DTOs
{
    public class UpdateUserDto
    {
        /// <summary>
        /// User's name
        /// </summary>
        public string? Name { get; set; } = string.Empty;
        /// <summary>
        /// User's lastname
        /// </summary>
        public string? Surename { get; set; } = string.Empty;
        /// <summary>
        /// User's email
        /// </summary>
        public string? Email { get; set; } = string.Empty;
        /// <summary>
        /// User's password
        /// </summary>
        public string? Password { get; set; } = string.Empty;

    }
}