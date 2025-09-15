using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductCRUD.Data;
using ProductCRUD.Model;
using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProductCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ProductDbContext _dbContext; 

        public AuthController(IConfiguration config, ProductDbContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if (login.UserName == "admin" && login.Password == "password") 
            {
                var accessToken = GenerateJwtToken(login.UserName);
                var refreshToken = GenerateRefreshToken();

                var token = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = "admin",
                    ExpiryDate = DateTime.UtcNow.AddMinutes(10), 
                    IsRevoked = false
                };

                // Save refresh token to DB
                _dbContext.RefreshTokens.Add(token);
                _dbContext.SaveChanges();

                return Ok(new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            return Unauthorized();
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] TokenResponse request)
        {
            // Validate refresh token from DB
            var storedToken = _dbContext.RefreshTokens
                .FirstOrDefault(x => x.Token == request.RefreshToken && !x.IsRevoked);

            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return Unauthorized("Invalid refresh token");
            }

            var newAccessToken = GenerateJwtToken(storedToken.UserId);
            var newRefreshToken = GenerateRefreshToken();

            // Mark old refresh token revoked
            storedToken.IsRevoked = true;

            // Save new refresh token
            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = storedToken.UserId,
                ExpiryDate = DateTime.UtcNow.AddMinutes(10),
                IsRevoked = false
            });
            _dbContext.SaveChanges();

            return Ok(new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
