using System;

namespace Artoo.Models
{
    public class ArtooConfiguration : BaseEntity
    {
        public int ArtooConfigurationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public int Scope { get; set; }
        public DateTime DateRegister { get; set; }
    }
}
