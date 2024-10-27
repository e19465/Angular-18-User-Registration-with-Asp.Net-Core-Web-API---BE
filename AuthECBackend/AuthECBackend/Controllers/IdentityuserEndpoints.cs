
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthECBackend.Config;
using AuthECBackend.Models.DTO;
using AuthECBackend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthECBackend.Controllers
{
    public static class IdentityuserEndpoints
    {
        public static IEndpointRouteBuilder MapIdentityuserEndpoints(this IEndpointRouteBuilder app)
        {
            // User registration endpoint
            app.MapPost("/signup", RegisterUser);
            
            // User login endpoint
            app.MapPost("/signin",LoginUser);

            return app;
        }

        private static async Task<IResult> RegisterUser(
            UserManager<AppUser> userManager, 
            UserRegisterDTO userRegisterDTO)
            {
                // Check if the password and confirm password match
                if (userRegisterDTO.Password != userRegisterDTO.ConfirmPassword)
                {
                    return Results.BadRequest(new { Message = "Password and Confirm Password do not match." });
                }

                // Trim whitespace from fullName and check if it is not empty or just whitespace
                if (string.IsNullOrWhiteSpace(userRegisterDTO.FullName?.Trim()))
                {
                    return Results.BadRequest(new { Message = "Full name is required and cannot be blank." });
                }

                AppUser user = new AppUser
                {
                    FullName = userRegisterDTO.FullName.Trim(),
                    Email = userRegisterDTO.Email,
                    UserName = userRegisterDTO.Email
                };

                var result = await userManager.CreateAsync(user, userRegisterDTO.Password);

                if (result.Succeeded)
                {
                    return Results.Ok(result);
                }
                else
                {
                    return Results.BadRequest(result.Errors);
                }
            }

        private static async Task<IResult> LoginUser(
            UserManager<AppUser> userManager,
            UserLoginDTO userLoginDTO,
            IOptions<AppSettings> appSettings
            )
            {
                var foundUser = await userManager.FindByEmailAsync(userLoginDTO.Email);
                if (foundUser != null && await userManager.CheckPasswordAsync(foundUser, userLoginDTO.Password))
                {
                    var accessSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.AccessTokenSecret));
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim("UserId", foundUser.Id)
                            }),
                        Expires = DateTime.UtcNow.AddMinutes(5), // token expires in 5 minutes
                        SigningCredentials = new SigningCredentials(accessSecret, SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);
                    return Results.Ok(new { accessToken = token });
                }
                else
                {
                    return Results.BadRequest(new { Message = "Invalid email or password." });
                }
            }
    }
}
