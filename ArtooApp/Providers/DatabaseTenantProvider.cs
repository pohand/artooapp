using Artoo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Artoo.Providers
{
    public class DatabaseTenantProvider : ITenantProvider
    {
        private string _host;
        private string _subdomain;
        private static IList<Tenant> _tenants = new List<Tenant>()
        {
            new Tenant { TenantId = 3, Name = "dpc", HostName="dpc", ConnectionString = "ConnStr1" },
            new Tenant { TenantId = 4, Name = "garmex", HostName="garmex", ConnectionString = "ConnStr2" },
        };

        public DatabaseTenantProvider(IHttpContextAccessor accessor)
        {
            var fullAddress = accessor.HttpContext?.Request?
                .Headers?["Host"].ToString()?.Split('.');
            if (fullAddress?.Length < 2)
            {
                //actionExecutingContext.Result = new StatusCodeResult(404);
                //base.OnActionExecuting(actionExecutingContext);
            }
            else
            {
                _subdomain = fullAddress?[0];
                //var tenant = _dbcontext.Tenants
                //    .SingleOrDefault(t => string.Equals(t.HostName, subdomain, StringComparison.OrdinalIgnoreCase));
            }

             _host = accessor?.HttpContext?.Request?.Host.Value;
        }

        public Tenant GetTenant()
        {
            return _tenants.FirstOrDefault(x => x.HostName == _subdomain);
        }
    }
}
