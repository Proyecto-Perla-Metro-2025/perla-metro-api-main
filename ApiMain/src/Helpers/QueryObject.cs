using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersService.src.Helper
{
    public class QueryObject
    {
        /// <summary>
        /// User's name
        /// </summary>
        public string? Name { get; set; } = string.Empty;
        /// <summary>
        /// User's lastname
        /// </summary>
        public string? Email { get; set; } = string.Empty;
        /// <summary>
        /// User's state
        /// </summary>
        public bool? isActive { get; set; }
    }
}

        