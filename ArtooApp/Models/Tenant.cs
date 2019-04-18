using System;

namespace Artoo.Models
{
    public class Tenant
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public DateTime? CreationTime { get; set; }
        public string Description { get; set; }
        public string HostName { get; set; }
        public string ConnectionString { get; set; }
    }
}
