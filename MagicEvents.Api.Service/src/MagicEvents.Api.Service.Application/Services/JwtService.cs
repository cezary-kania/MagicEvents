using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MagicEvents.Api.Service.Application.Auth.interfaces;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using MagicEvents.Api.Service.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace MagicEvents.Api.Service.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IJwtSettings _jwtSettings;
        public JwtService(IJwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }
        public AuthTokenDto GenerateToken(User user)
        {
            Claim[] claims = GetCredentials(user);
            var currentTime = DateTime.UtcNow;
            var expires = currentTime.Add(_jwtSettings.TokenLifetime);
            var secret = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256
            );
            var securityToken = GetSecurityToken(claims, currentTime, expires, signingCredentials);
            var jwt = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return new AuthTokenDto 
            {
                Token = jwt,
                Expiry = expires
            };
        }

        private static JwtSecurityToken GetSecurityToken(Claim[] claims, DateTime currentTime, DateTime expires, SigningCredentials signingCredentials)
        {
            return new JwtSecurityToken(
                            claims: claims,
                            notBefore: currentTime,
                            expires: expires,
                            signingCredentials: signingCredentials
                        );
        }

        private static Claim[] GetCredentials(User user)
        {
            return new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Identity.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()),
            };
        }
    }
}