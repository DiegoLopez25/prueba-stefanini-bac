using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace WEB.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _roles = roles ?? Array.Empty<string>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var token = http.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            var tipo = http.Session.GetString("TipoUsuario") ?? string.Empty;
            if (_roles.Length > 0)
            {
                var allowed = _roles.Any(r => string.Equals(r, tipo, StringComparison.OrdinalIgnoreCase));
                if (!allowed)
                {
                    context.Result = new RedirectToActionResult("Index", "Home", new { area = "" });
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
