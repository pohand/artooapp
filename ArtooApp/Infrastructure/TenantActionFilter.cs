using Artoo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Infrastructure
{
    public class TenantAttribute : ActionFilterAttribute
    {
        private readonly AppDbContext _dbcontext;
        public TenantAttribute(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            var fullAddress = actionExecutingContext.HttpContext?.Request?
                .Headers?["Host"].ToString()?.Split('.');
            if (fullAddress.Length < 2)
            {
                actionExecutingContext.Result = new StatusCodeResult(404);
                base.OnActionExecuting(actionExecutingContext);
            }
            else
            {
                var subdomain = fullAddress[0];
                var tenant = _dbcontext.Tenants
                    .SingleOrDefault(t => string.Equals(t.HostName, subdomain, StringComparison.OrdinalIgnoreCase));
                if (tenant != null)
                {
                    actionExecutingContext.RouteData.Values.Add("tenant", tenant);
                    base.OnActionExecuting(actionExecutingContext);
                }
                else
                {
                    actionExecutingContext.Result = new StatusCodeResult(404);
                    base.OnActionExecuting(actionExecutingContext);
                }
            }
        }
    }
}
