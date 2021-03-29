using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebServer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace WebServer.Services
{
    public class UserService
    {
        private IConfiguration Configuration { get; }
        private readonly LoginConnector loginConnector;
        private readonly ILogger _logger;

        public UserService(IConfiguration configuration)//, ILogger logger)
        {
            Configuration = configuration;
            //_logger = logger;
            loginConnector = new LoginConnector();
        }

        public string LoginUser(string userId, string password)
        {
            var userAccess = loginConnector.Find(userId);
            if (userAccess.ReturnStatus != ReturnStatus.Ok) return null;

            var key = Encoding.ASCII.GetBytes(Configuration["JWT:Secret"]);
                
            var JWToken = new JwtSecurityToken(
                issuer: "http://localhost:5001/",
                audience: "http://localhost:5001/",
                claims: GetUserClaims(userAccess.Value),
                notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
            return token;
        }

        private IEnumerable<Claim> GetUserClaims(UserModel user)
        {
            IEnumerable<Claim> claims = new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                    };
            return claims;
        }

        private string GenerateJwtToken(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["JWT:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim("Username", user.UserName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
