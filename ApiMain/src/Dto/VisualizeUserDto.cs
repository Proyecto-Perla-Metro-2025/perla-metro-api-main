using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMain.src.DTOs
{
    public class VisualizeUserDto
    {
        /// <summary>
        /// User's Id
        /// </summary>
        public required string Id { get; set; }
         /// <summary>
        /// User's fullname (name + surename)
        /// </summary>
        public required string Fullname { get; set; }
        /// <summary>
        /// User's email
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// User's state
        /// </summary>
        public bool isActive { get; set; }
        /// <summary>
        /// User's registration date
        /// </summary>
        public DateOnly RegistrationDate { get; set; }
    }
}