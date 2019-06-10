using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Models
{
    public class AppUserManager : UserManager<ApplicationUser>
    {
        public AppUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
                                IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators,
                                IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
                                IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger)
                                : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger) { }

        public virtual Task<ApplicationUser> FindByNameAndTenantAsync(string userName, int tenantId)
        {
            return Task.FromResult(base.Users.Where(u => u.NormalizedUserName == userName.ToUpper().Trim() && u.TenantId == tenantId).SingleOrDefault());
        }

    }
}
