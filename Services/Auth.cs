using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class Auth
    {
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;
        public Auth(IConfiguration configuration)
        {
            _configuration = configuration;
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true, // Validate token expiry
                ValidIssuer = "yourIssuer",
                ValidAudience = "yourAudience",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSecretKey"))
            };
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPassowrd(string password, byte[] passwordHash, byte[] passwordSalt) 
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public string CreateToken(string data, string role)
        {

            List<Claim> claims =
            [
                role != "admin" ? new Claim(ClaimTypes.Email, data) : new Claim(ClaimTypes.Name, data),
                new Claim(ClaimTypes.Role, role),
            ];

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.Now.AddMinutes(2));
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        /*private readonly TokenValidationParameters _tokenValidationParameters;

        public TokenService()
        {
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true, // Validate token expiry
                ValidIssuer = "yourIssuer",
                ValidAudience = "yourAudience",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSecretKey"))
            };
        }*/

        public bool IsTokenValid(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

                // Check token expiry
                if (validatedToken.ValidTo < DateTime.UtcNow)
                {
                    return false; // Token is expired
                }

                // Token is valid
                return true;
            }
            catch (Exception)
            {
                // Token validation failed
                return false;
            }
        }
    }
}
