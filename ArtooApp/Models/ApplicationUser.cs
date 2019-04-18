using Microsoft.AspNetCore.Identity;

namespace Artoo.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? FactoryId { get; set; }
        public virtual Factory Factory { get; set; }
        public  int? TechManagerId { get; set; }
        public virtual TechManager TechManager { get; set; }
        public int? TenantId { get; set; }
    }
}
