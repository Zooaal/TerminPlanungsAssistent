using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebServer.Services
{
    // Eigenes Authorize Attribute für die API
    public class AuthorizedAttribute : TypeFilterAttribute
    {
        public AuthorizedAttribute(params string[] claim) : base(typeof(AuthorizedFilter))
        {
            Arguments = new object[] { claim };
        }
    }
    public class AuthorizedFilter : IAuthorizationFilter
    {
        readonly string[] _claim;

        public AuthorizedFilter(params string[] claim)
        {
            _claim = claim;
        }
        // Bei jeder Anfrage wird diese Methode durchgegangen
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Überprüfen auf Authorisierung
            bool isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            // Wenn nicht Authorisiert zurück zur Login Seite
            if (!isAuthenticated)
            { 
                context.Result = new RedirectResult("~/Home/LoginView");
            }
            else if(_claim.Length != 0)
            {
                bool flagClaim = false;
                foreach (var item in _claim)
                {
                    if (context.HttpContext.User.HasClaim(item, item))
                        flagClaim = true;
                }
                if (!flagClaim)
                {
                    context.Result = new RedirectResult("~/Home/LoginView");
                }
            }
        }
    }
}
