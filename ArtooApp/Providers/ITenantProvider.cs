using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Providers
{
    public interface ITenantProvider
    {
        Tenant GetTenant();
    }
}
