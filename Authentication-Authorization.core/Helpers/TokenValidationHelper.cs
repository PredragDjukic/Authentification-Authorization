using Authentication_Authorization.BLL.Exceptions;
using Authentication_Authorization.BLL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace Authentication_Authorization.BLL.Helpers
{
    public static class TokenValidationHelper
    {
        public static void CheckTokenForServerId(int selectedId, HttpContext context)
        {
            if (context != null)
            {
                var claims = context.User.Claims;
                foreach (var claim in claims)
                {
                    if (claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                    {
                        if (claim.Value == "Admin")
                        {
                            return;
                        }
                    }

                    if (claim.Type == "id")
                    {
                        if (Convert.ToInt32(claim.Value) == selectedId)
                        {
                            return;
                        }
                    }
                }
            }

            throw new BussinesException("Invalid user", 400);
        }

        public static string GetUsernameFromJwt(JwtModel token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token.Token);
            var encryptedToken = jsonToken as JwtSecurityToken;

            var claims = encryptedToken.Claims;
            foreach (var claim in claims)
            {
                if (claim.Type == "username")
                {
                    return claim.Value;
                }
            }

            return String.Empty;
        }
    }
}
