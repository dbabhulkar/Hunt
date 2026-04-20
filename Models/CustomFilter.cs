using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API_HUNT.Models
{
    /// <summary>
    /// Marks a controller or action as exempt from the CustomFilter session check.
    /// Apply this only to endpoints that must be accessible without authentication (e.g., Login).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SkipCustomFilterAttribute : Attribute { }

    public class CustomFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                bool skip = descriptor.MethodInfo
                    .GetCustomAttributes(typeof(SkipCustomFilterAttribute), false).Any()
                    || descriptor.ControllerTypeInfo
                    .GetCustomAttributes(typeof(SkipCustomFilterAttribute), false).Any();

                if (skip)
                {
                    base.OnActionExecuting(context);
                    return;
                }
            }

            var session = context.HttpContext.Session;
            var role = session.GetString("Role");
            if (string.IsNullOrEmpty(role))
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
