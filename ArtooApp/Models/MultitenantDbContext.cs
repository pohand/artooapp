using Artoo.Providers;
using Microsoft.EntityFrameworkCore;

namespace Artoo.Models
{
    public class MultitenantDbContext
    {
        //public static int Tenant1Id = 1;
        //public static int Tenant2Id = 2;
        //private ITenantProvider _tenantProvider;

        //public DbSet<Mistake> Mistakes { get; set; }
        //public DbSet<PassionBrand> PassionBrands { get; set; }
        //public DbSet<Factory> Factories { get; set; }
        //public DbSet<Inspection> Inspections { get; set; }
        //public DbSet<Email> Emails { get; set; }
        //public DbSet<InspectionMistakeDetail> InspectionMistakeDetails { get; set; }
        //public DbSet<EmailRule> EmailRules { get; set; }
        //public DbSet<FinalWeek> FinalWeeks { get; set; }
        //public DbSet<TechManager> TechManagers { get; set; }
        //public DbSet<EmailRuleDetail> EmailRuleDetails { get; set; }

        //public MultitenantDbContext(DbContextOptions<MultitenantDbContext> options,
        //                            ITenantProvider tenantProvider) : base(options)
        //{
        //    _tenantProvider = tenantProvider;
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Mistake>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenantId());
        //    modelBuilder.Entity<PassionBrand>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenantId());
        //    modelBuilder.Entity<Factory>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenantId());
        //    modelBuilder.Entity<Inspection>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenantId());
        //    modelBuilder.Entity<Email>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenantId());
        //    modelBuilder.Entity<EmailRule>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenantId());
        //    modelBuilder.Entity<TechManager>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenantId());
        //    modelBuilder.Entity<EmailRuleDetail>().HasQueryFilter(p => p.TenantId == _tenantProvider.GetTenantId());
        //}

        //public void AddSampleData() 
        //{
        //    People.Add(new Person
        //    {
        //        Id = Guid.Parse("79865406-e01b-422f-bd09-92e116a0664a"),
        //        TenantId = Tenant1Id,
        //        FirstName = "Gunnar",
        //        LastName = "Peipman"
        //    });

        //    People.Add(new Person
        //    {
        //        Id = Guid.Parse("d5674750-7f6b-43b9-b91b-d27b7ac13572"),
        //        TenantId = Tenant2Id,
        //        FirstName = "John",
        //        LastName = "Doe"
        //    });

        //    People.Add(new Person
        //    {
        //        Id = Guid.Parse("e41446f9-c779-4ff6-b3e5-752a3dad97bb"),
        //        TenantId = Tenant1Id,
        //        FirstName = "Mary",
        //        LastName = "Jones"
        //    });

        //    SaveChanges();
        //}
    }
}
