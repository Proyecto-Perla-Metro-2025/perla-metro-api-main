using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMain.src.Dto
{
    public class UserDto
    {
        /// <summary>
        /// User's id
        /// </summary>
        public required string Id { get; set; }
        /// <summary>
        /// User's email
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// User's name
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// User's surename
        /// </summary>
        public required string SureName { get; set; }
        /// <summary>
        /// User's role
        /// </summary>
        public required string Role { get; set; }
        /// <summary>
        /// Date of creation
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// User's state
        /// </summary>
        public bool IsActive { get; set; }
    }
}