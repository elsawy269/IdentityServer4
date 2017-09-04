using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMangement.API.Authorization
{
    public class MustOwnImageHandler : AuthorizationHandler<MustOwnImageRequirements>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            MustOwnImageRequirements requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;

            if (filterContext==null)
            {
                context.Fail();
                return Task.FromResult(0);
            }

         var id=   filterContext.RouteData.Values["id"].ToString();
            /// any codation can check then run 
            /// 
            //if (id == null)
            //{
            //    context.Fail();
            //    return Task.FromResult(0);
            //}

            //can access claims like this

            var ownerId= context.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;



            context.Succeed(requirement);
            return Task.FromResult(0);

        }
    }
}
