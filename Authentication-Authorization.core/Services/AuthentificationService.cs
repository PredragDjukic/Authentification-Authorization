using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.Exceptions;
using Authentication_Authorization.BLL.Helpers;
using Authentication_Authorization.BLL.Models;
using Authentication_Authorization.DAL.DatabaseAccess;
using Authentication_Authorization.DAL.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authentication_Authorization.BLL.Services
{
    public class AuthentificationService : IAuthentificationService
    {
        private readonly IOptions<JwtConfigurationsModel> _jwtConfig;
        private readonly IOptions<DatabaseConnectionStringModel> _connectionString;
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepository;
        private readonly RefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserService _userService;

        public AuthentificationService(
            IOptions<JwtConfigurationsModel> jwtConfig,
            IMapper mapper,
            IOptions<DatabaseConnectionStringModel> connectionString,
            IRefreshTokenService refreshTokenService,
            IUserService userService
        )
        {
            _jwtConfig = jwtConfig;
            _mapper = mapper;
            _userRepository = new UserRepository();
            _connectionString = connectionString;
            _refreshTokenRepository = new RefreshTokenRepository();
            _refreshTokenService = refreshTokenService;
            _userService = userService;
        }

        public string Refresh(JwtModel jwtModel, HttpRequest request, HttpResponse response)
        {
            RefreshToken fetchedRefreshToken = _refreshTokenService.GetRefreshTokenFromUser(jwtModel);
            RefreshToken refreshTokenFromCookie = this.GetRefreshTokenFromCookie(request);

            if(fetchedRefreshToken.Username != refreshTokenFromCookie.Username)
                throw new BussinesException("Refresh tokens doesn't match", 400);

            UserForTokenDTO principalForNewToken =  _userService.FindUserByUsername(refreshTokenFromCookie.Username).Result;
            string newJwtToken = GenerateJwt(principalForNewToken);
            this.AddRefreshTokenToCookieAndUpdateDatabase(principalForNewToken.Username, response);
            return newJwtToken;

            
        }

        private RefreshToken GetRefreshTokenFromCookie(HttpRequest request)
        {
            return new RefreshToken()
            {
                Username = request.Cookies["User"],
                Expiration = Convert.ToDateTime(request.Cookies["Expiration"]),
                Token = request.Cookies["RefreshToken"]
            };
        }

        public string ValidatePrincipalAndGenerateTokens(PrincipalModel principal, HttpResponse response)
        {
            UserForTokenDTO principalForToken = this.Validate(principal);
            string jwtToken = GenerateJwt(principalForToken);
            this.AddRefreshTokenToCookieAndDatabase(principal.Username, response);

            return jwtToken;
        }

        private UserForTokenDTO Validate(PrincipalModel principal)
        {
            User userToValidate = _userRepository.GetUserByUsername(
                principal.Username,
                _connectionString.Value.ConnectionString
            );

            if (userToValidate.Username == null)
                throw new BussinesException("Username doesn't exist", 400);

            if (!PasswordHashHelper.VerifyPassword(principal.Username, userToValidate.Password))
                throw new BussinesException("Password incorrect", 400);

            return _mapper.Map<UserForTokenDTO>(userToValidate);
        }

        public string GenerateJwt(UserForTokenDTO principal)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Value.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = this.CreateJwtSecurityToken(principal, credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken CreateJwtSecurityToken(UserForTokenDTO principal, SigningCredentials credentials)
        {
            return new JwtSecurityToken(
                _jwtConfig.Value.Issuer,
                _jwtConfig.Value.Issuer,
                this.GenerateClaims(principal),
                expires: DateTime.UtcNow.AddMinutes(_jwtConfig.Value.Expiration),
                signingCredentials: credentials
            );
        }

        private Claim[] GenerateClaims(UserForTokenDTO principal)
        {
            return new Claim[]
            {
                new Claim("role", principal.Role),
                new Claim("username", principal.Username),
                new Claim("id", principal.Id.ToString())
            };
        }

        private void AddRefreshTokenToCookieAndDatabase(string username, HttpResponse response)
        {
            RefreshToken existingValidRefreshToken = _refreshTokenRepository.GetValidRefreshToken(
                username,
                _connectionString.Value.ConnectionString
            );

            if (existingValidRefreshToken.Username == null)
            {
                this.CheckIfInvalidRefreshTokenExist(username, response);
            } 
            else
            {
                this.AddExistingRefreshTokenToCookie(existingValidRefreshToken, response);
            }
            
        }

        private void CheckIfInvalidRefreshTokenExist(string username, HttpResponse response)
        {
            RefreshToken existingInvalidRefreshToken = _refreshTokenRepository.GetInvalidRefreshToken(
                username,
                _connectionString.Value.ConnectionString
            );

            if (existingInvalidRefreshToken.Username == null)
            {
                RefreshToken newRefreshToken = this.GenerateAndAddNewRefreshTokenToCookie(username, response);
                _refreshTokenRepository.AddRefreshToken(newRefreshToken, _connectionString.Value.ConnectionString);
            }
            else
            {
                RefreshToken updatedRefreshToken = GenerateRefreshToken(existingInvalidRefreshToken.Username);
                _refreshTokenRepository.UpdateRefreshToken(updatedRefreshToken, _connectionString.Value.ConnectionString);
            }
        }

        private RefreshToken GenerateAndAddNewRefreshTokenToCookie(string username, HttpResponse response)
        {
            var newRefreshToken = GenerateRefreshToken(username);
            this.AddExistingRefreshTokenToCookie(newRefreshToken, response);

            return newRefreshToken;
        }

        private void AddRefreshTokenToCookieAndUpdateDatabase(string username, HttpResponse response)
        {
            RefreshToken updatedToken = GenerateRefreshToken(username);
            _refreshTokenRepository.UpdateRefreshToken(updatedToken, _connectionString.Value.ConnectionString);
            this.AddExistingRefreshTokenToCookie(updatedToken, response);
        }

        public RefreshToken GenerateRefreshToken(string username)
        {
            RefreshToken refreshToken = new()
            {
                Token = this.GenerateRefreshTokenRandomSequence(),
                Expiration = DateTime.UtcNow.AddHours(24),
                Username = username
            };

            return refreshToken;
        }

        private string GenerateRefreshTokenRandomSequence()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private void AddExistingRefreshTokenToCookie(RefreshToken refreshToken, HttpResponse response)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddHours(3),
            };

            response.Cookies.Append(
                    "RefreshToken",
                    refreshToken.Token,
                    cookieOptions);

            response.Cookies.Append(
                    "Expiration",
                    refreshToken.Expiration.ToString(),
                    cookieOptions);

            response.Cookies.Append(
                    "User",
                    refreshToken.Username,
                    cookieOptions);
        }


    }
}
