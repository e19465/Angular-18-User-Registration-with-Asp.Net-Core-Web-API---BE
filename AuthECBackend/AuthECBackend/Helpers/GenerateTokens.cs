using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthECBackend.Config;
using AuthECBackend.Data;
using AuthECBackend.Models.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthECBackend.Helpers
{
    public static class GenerateTokens
    {
        public static string GenerateAccessToken(AppUser user, IOptions<AppSettings> appSettings, object userRole)
        {
            // providing access token
            var accessSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.AccessTokenSecret));

            ClaimsIdentity claims = new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", user.Id),
                new Claim("Gender", user.Gender!.ToString()),
                new Claim("Age", ((DateTime.Now.Year - user.DOB.Year)).ToString()),
                //new Claim(ClaimTypes.Role, userRole)
            });
            if (user.LibraryId != null)
            {
                claims.AddClaim(new Claim("LibraryId", user.LibraryId.ToString()!));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddSeconds(30), // token expires in 5 minutes
                SigningCredentials = new SigningCredentials(accessSecret, SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return token;
        }

        public static async Task<string> GenerateRefreshToken(AppUser user, IOptions<AppSettings> appSettings, ApplicationDbContext dbContext)
        {
            // providing access token
            var refreshSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.RefreshTokenSecret));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                                    new Claim("UserId", user.Id)
                    }),
                Expires = DateTime.UtcNow.AddDays(7), // token expires in 7 days
                SigningCredentials = new SigningCredentials(refreshSecret, SecurityAlgorithms.HmacSha256Signature),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            var refreshToken = new RefreshTokensEntity
            {
                RefreshToken = token,
                IsUsed = false
            };

            await dbContext.RefreshTokens.AddAsync(refreshToken);
            await dbContext.SaveChangesAsync();

            return token;
        }
    }
}
