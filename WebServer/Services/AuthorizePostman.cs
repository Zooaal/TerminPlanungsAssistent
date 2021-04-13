using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebServer.Services
{
    public class AuthorizePostmanAttribute : TypeFilterAttribute
    {
        public AuthorizePostmanAttribute(params string[] claim) : base(typeof(AuthorizePostmanFilter))
        {
            Arguments = new object[] { claim };
        }
    }
    public class AuthorizePostmanFilter : IAuthorizationFilter
    {
        readonly string[] _claim;

        public AuthorizePostmanFilter(params string[] claim)
        {
            _claim = claim;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                context.Result = new UnauthorizedResult();
            }
            else if (_claim.Length != 0)
            {
                bool flagClaim = false;
                foreach (var item in _claim)
                {
                    if (context.HttpContext.User.HasClaim(item, item))
                        flagClaim = true;
                }
                if (!flagClaim)
                {
                    context.Result = new UnauthorizedResult();
                }
            }
        }
    }
}
