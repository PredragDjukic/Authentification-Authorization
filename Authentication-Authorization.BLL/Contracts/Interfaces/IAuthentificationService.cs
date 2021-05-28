using Authentication_Authorization.BLL.Models;
using Authentication_Authorization.DAL.Entities;
using Microsoft.AspNetCore.Http;

namespace Authentication_Authorization.BLL.Contracts.Interfaces
{
    public interface IAuthentificationService
    {
        string ValidatePrincipalAndGenerateTokens(PrincipalModel principal, HttpResponse response);
        string GenerateJwt(UserForTokenDTO principal);
        public RefreshToken GenerateRefreshToken(string username);
        string Refresh(JwtModel jwtModel, HttpRequest request, HttpResponse response);

    }
}
