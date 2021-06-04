using Authentication_Authorization.BLL.Contracts.Enums;
using Authentication_Authorization.BLL.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;

namespace Authentication_Authorization.Presentation.Attributes
{
    public class IsAuthorizedAttribute : Attribute, IAuthorizationFilter
    {
        private readonly Roles role;


        public IsAuthorizedAttribute(Roles role)
        {
            this.role = role;
        }


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Claim userRoleClaim = context.HttpContext.User.Claims.FirstOrDefault(e => e.Type == "titleRole");

            if (userRoleClaim == null)
                throw new BussinesException("Unauthorized", 401);

            int userRole = Convert.ToInt32(userRoleClaim.Value);

            if (Convert.ToInt32(role) > userRole)
                throw new BussinesException("Forbidden", 403);
        }
    }
}
