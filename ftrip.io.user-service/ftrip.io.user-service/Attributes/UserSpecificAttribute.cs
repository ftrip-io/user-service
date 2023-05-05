using ftrip.io.framework.Contexts;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Attributes
{
    public class UserSpecificAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userIdFromRoute = GetUserIdFromRoute(context);
            var userIdFromToken = GetUserIdFromToken(context);

            if (userIdFromRoute != userIdFromToken)
            {
                throw new ForbiddenException("Sorry, you are not allowed to perform this action!");
            }

            await next();
        }

        private string GetUserIdFromRoute(ActionExecutingContext context)
        {
            return context.HttpContext.Request.RouteValues["userId"]?.ToString() ?? null;
        }

        private string GetUserIdFromToken(ActionExecutingContext context)
        {
            var currentUserContext = context.HttpContext.RequestServices.GetService(typeof(CurrentUserContext)) as CurrentUserContext;

            return currentUserContext.Id;
        }
    }
}