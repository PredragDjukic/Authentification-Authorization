using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.Exceptions;
using Authentication_Authorization.BLL.Helpers;
using Authentication_Authorization.BLL.Models;
using Authentication_Authorization.DAL.DatabaseAccess;
using Authentication_Authorization.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Authentication_Authorization.BLL.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly RefreshTokenRepository _refreshTokenRepository;
        private readonly IOptions<DatabaseConnectionStringModel> _connectionStringOptions;

        public RefreshTokenService(IOptions<DatabaseConnectionStringModel> connectionStringOptions)
        {
            _refreshTokenRepository = new RefreshTokenRepository();
            _connectionStringOptions = connectionStringOptions;
        }

        public RefreshToken GetRefreshTokenFromUser(JwtModel token)
        {
            string username = TokenValidationHelper.GetUsernameFromJwt(token);
            RefreshToken validRefreshToken = _refreshTokenRepository.GetValidRefreshToken(
                username,
                _connectionStringOptions.Value.ConnectionString
            );

            if (validRefreshToken.Username == null)
                throw new BussinesException("Refresh token doesn't exist in database", 400);

            return validRefreshToken;
        }

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            _refreshTokenRepository.AddRefreshToken(
                refreshToken,
                _connectionStringOptions.Value.ConnectionString
            );
        }

        public void UpdateRefreshToken(RefreshToken updatedRefreshToken)
        {
            _refreshTokenRepository.UpdateRefreshToken(
                updatedRefreshToken, 
                _connectionStringOptions.Value.ConnectionString
            );
        }
    }
}
