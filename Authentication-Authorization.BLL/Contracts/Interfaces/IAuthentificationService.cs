using Authentication_Authorization.BLL.DTOs.UserDTOs;
using Authentication_Authorization.BLL.Models;

namespace Authentication_Authorization.BLL.Contracts.Interfaces
{
    public interface IAuthentificationService
    {
        string ValidatePrincipalAndGenerateToken(PrincipalModel principal);
        string GenerateJwt(UserForTokenDTO principal);
        string Refresh(JwtModel jwtModel);
    }
}
