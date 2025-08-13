using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Model.Data;
using Model.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Model.Helper
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly PracticeSkillContext _dbContext;
        
        public JwtHelper(IConfiguration configuration, PracticeSkillContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public string GenerateJwtToken(SystemUserAccount account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.AccountId),
                new Claim(JwtRegisteredClaimNames.Email, account.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Role, account.RoleId.ToString()),
                new Claim("userId", account.AccountId)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:ValidIssuer"],
                Audience = _configuration["Jwt:ValidAudience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:ValidIssuer"],
                    ValidAudience = _configuration["Jwt:ValidAudience"],
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return true;

            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }

        public string? GetClaimValue(string token, string claimType)
        {
            var principal = ValidateToken(token);
            if (principal == null)
                return null;

            var claim = principal.FindFirst(claimType);
            return claim?.Value;
        }

        public Dictionary<string, string> GetAllClaims(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null)
                return new Dictionary<string, string>();

            return principal.Claims.ToDictionary(c => c.Type, c => c.Value);
        }

        public DateTime GetTokenExpirationTime(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                throw new ArgumentException("Invalid token format");

            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
    }
}
