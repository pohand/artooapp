using Artoo.Common;
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
            new Tenant { TenantId = (int)TenantEnum.dpc, Name = TenantEnum.dpc.ToString(), HostName=TenantEnum.dpc.ToString() },
            new Tenant { TenantId = (int)TenantEnum.garmex, Name = TenantEnum.garmex.ToString(), HostName="garmex" },
            new Tenant { TenantId = (int)TenantEnum.tng, Name = TenantEnum.tng.ToString(), HostName=TenantEnum.tng.ToString() }
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
