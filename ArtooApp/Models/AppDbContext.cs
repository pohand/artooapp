using Artoo.Providers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Artoo.Models
{
    public class AppDbContext: IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        private ITenantProvider _tenantProvider;
        private int? _tenantId;
        public AppDbContext(DbContextOptions<AppDbContext> options,
                                    ITenantProvider tenantProvider) : base(options)
        {
            _tenantProvider = tenantProvider;
            _tenantId = tenantProvider?.GetTenant()?.TenantId;
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Mistake>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenant().TenantId);
            modelBuilder.Entity<PassionBrand>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenant().TenantId);
            modelBuilder.Entity<Factory>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenant().TenantId);
            modelBuilder.Entity<Inspection>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenant().TenantId);
            modelBuilder.Entity<Email>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenant().TenantId);
            modelBuilder.Entity<EmailRule>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenant().TenantId);
            modelBuilder.Entity<TechManager>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenant().TenantId);
            modelBuilder.Entity<EmailRuleDetail>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenant().TenantId);
            modelBuilder.Entity<MistakeFree>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenant().TenantId);

            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenant().TenantId);
        }

        public DbSet<Mistake> Mistakes { get; set; }
        public DbSet<PassionBrand> PassionBrands { get; set; }
        public DbSet<Factory> Factories { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<InspectionMistakeDetail> InspectionMistakeDetails { get; set; }
        public DbSet<EmailRule> EmailRules { get; set; }
        public DbSet<FinalWeek> FinalWeeks { get; set; }
        public DbSet<TechManager> TechManagers { get; set; }
        public DbSet<EmailRuleDetail> EmailRuleDetails { get; set; }
        public DbSet<MistakeFree> MistakeFrees { get; set; }

        public override int SaveChanges()
        {
            var entries = this
                   .ChangeTracker
                   .Entries<BaseEntity>()
                   .ToList();

            foreach (var entityEntry in entries)
            {
                entityEntry.Property(m => m.TenantId).CurrentValue = _tenantId;
            }

            return base.SaveChanges();
        }

    }
}
