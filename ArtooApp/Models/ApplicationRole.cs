using Microsoft.AspNetCore.Identity;
using System;

namespace Artoo.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
