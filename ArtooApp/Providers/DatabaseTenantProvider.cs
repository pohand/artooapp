using Artoo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using System;
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
        private static List<Tenant> _tenants = new List<Tenant>()
        {
            new Tenant { TenantId = 1, Name = "dpc", HostName="dpc" },
            new Tenant { TenantId = 2, Name = "garmex", HostName="garmex" },
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
            return _tenants.SingleOrDefault(t => string.Equals(t.HostName, _subdomain, StringComparison.OrdinalIgnoreCase));
        }

        public List<Tenant> GetTenants()
        {
            return _tenants;
        }
    }
}
