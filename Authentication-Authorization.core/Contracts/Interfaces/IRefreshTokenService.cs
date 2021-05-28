using Authentication_Authorization.BLL.Models;
using Authentication_Authorization.DAL.Entities;
using Microsoft.AspNetCore.Http;

namespace Authentication_Authorization.BLL.Contracts.Interfaces
{
    public interface IRefreshTokenService
    {
        RefreshToken GetRefreshTokenFromUser(JwtModel token);
        void AddRefreshToken(RefreshToken newRefreshToken);
        void UpdateRefreshToken(RefreshToken updatedRefreshToken);
    }
}
