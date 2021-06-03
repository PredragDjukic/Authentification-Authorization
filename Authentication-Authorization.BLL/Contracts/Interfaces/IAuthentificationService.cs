using Authentication_Authorization.BLL.DTOs.UserDTOs;
using Authentication_Authorization.BLL.Models;
using Microsoft.AspNetCore.Http;

namespace Authentication_Authorization.BLL.Contracts.Interfaces
{
    public interface IAuthentificationService
    {
        string ValidatePrincipalAndGenerateToken(PrincipalModel principal, HttpResponse response);
        string GenerateJwt(UserForTokenDTO principal);
        string Refresh(JwtModel jwtModel);
    }
}
