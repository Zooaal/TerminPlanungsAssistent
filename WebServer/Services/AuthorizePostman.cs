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
        // Bei jeder Postman Anfrage wird diese Methode durchgegangen
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Überprüfen auf Authorisierung
            bool isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            // Wenn nicht Authorisiert Rückgabe eines unauthoriserten Results
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
