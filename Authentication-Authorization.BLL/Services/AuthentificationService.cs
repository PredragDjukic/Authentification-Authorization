﻿using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.Exceptions;
using Authentication_Authorization.BLL.Helpers;
using Authentication_Authorization.BLL.Models;
using Authentication_Authorization.DAL.Entities;
using Authentication_Authorization.DAL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Authentication_Authorization.BLL.Services
{
    public class AuthentificationService : IAuthentificationService
    {
        private readonly IOptions<JwtConfigurationsModel> _jwtConfig;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public AuthentificationService(
            IOptions<JwtConfigurationsModel> jwtConfig,
            IMapper mapper,
            IUserRepository userRepository)
        {
            _jwtConfig = jwtConfig;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public string Refresh(JwtModel jwtModel)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwtModel.Token);
            var encryptedToken = jsonToken as JwtSecurityToken;

            Claim issuedAt = encryptedToken.Claims.FirstOrDefault(e => e.Type == "issuedAt");
            Claim username = encryptedToken.Claims.FirstOrDefault(e => e.Type == "username");

            if (issuedAt == null || username == null)
                throw new BussinesException("Invalid token", 400);

            this.CheckIfRefreshTokenIsValid(issuedAt.Value);

            return this.GenerateJwtWithUsername(username.Value);
        }

        private string GenerateJwtWithUsername(string username)
        {
            User userForToken = _userRepository.GetUserByUsername(username);
            string newJwt = GenerateJwt(_mapper.Map<UserForTokenDTO>(userForToken));

            return newJwt;
        }

        private void CheckIfRefreshTokenIsValid(string issuedAt)
        {
            DateTime issuedAtDate = DateTime.Parse(issuedAt);
            this.CheckIfIssuedAtPassed(issuedAtDate);
        }

        private void CheckIfIssuedAtPassed(DateTime issuedAt)
        {
            DateTime timeLimit = issuedAt.AddHours(24);

            if (DateTime.UtcNow > timeLimit)
                throw new BussinesException("RefreshToken has expired", 400);
        }

        public string ValidatePrincipalAndGenerateToken(PrincipalModel principal, HttpResponse response)
        {
            UserForTokenDTO principalForToken = this.Validate(principal);
            string jwtToken = GenerateJwt(principalForToken);

            return jwtToken;
        }

        private UserForTokenDTO Validate(PrincipalModel principal)
        {
            User userToValidate = _userRepository.GetUserByUsername(principal.Username);

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
                new Claim("id", principal.Id.ToString()),
                new Claim("issuedAt", DateTime.UtcNow.ToString())
            };
        }
    }
}
