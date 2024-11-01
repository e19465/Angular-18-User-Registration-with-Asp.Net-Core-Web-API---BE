
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Text.Json;
using AuthECBackend.Config;
using AuthECBackend.Data;
using AuthECBackend.Helpers;
using AuthECBackend.Models.DTO;
using AuthECBackend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthECBackend.Controllers
{
    public static class IdentityuserEndpoints
    {
        public static IEndpointRouteBuilder MapIdentityuserEndpoints(this IEndpointRouteBuilder app)
        {
            // User registration endpoint
            app.MapPost("/signup", RegisterUser).AllowAnonymous();

            // User login endpoint
            app.MapPost("/signin", LoginUser).AllowAnonymous();

            // Refresh token endpoint
            app.MapPost("/user/refresh", RefreshToken).AllowAnonymous();

            return app;
        }

        private static async Task<IResult> RegisterUser(
            UserManager<AppUser> userManager,
            UserRegisterDTO userRegisterDTO)
        {
            try
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
                    UserName = userRegisterDTO.Email,
                    Gender = userRegisterDTO.Gender,
                    DOB = DateOnly.FromDateTime(DateTime.Now.AddYears(-userRegisterDTO.Age)),
                    LibraryId = userRegisterDTO.LibraryId
                };

                var result = await userManager.CreateAsync(user, userRegisterDTO.Password);

                if (!result.Succeeded)
                {
                    // Gather error messages to send a more user-friendly response
                    return Results.BadRequest(result.Errors);
                }

                // Assign role to user
                var roleResult = await userManager.AddToRoleAsync(user, userRegisterDTO.Role);
                if (!roleResult.Succeeded)
                {
                    return Results.BadRequest(roleResult.Errors);
                }

                return Results.Ok(result);
            }
            catch (Exception ex)
            {

                // Send 500 internal server error with relevant exception data
                return Results.Problem(new
                {
                    Message = "An error occurred during registration.",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace // Only if you need to show stack trace in development
                }.ToString(), statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        private static async Task<IResult> LoginUser(
            UserManager<AppUser> userManager,
            UserLoginDTO userLoginDTO,
            IOptions<AppSettings> appSettings,
            ApplicationDbContext appDbContext
        )
        {
            try
            {
                var foundUser = await userManager.FindByEmailAsync(userLoginDTO.Email);
                if (foundUser != null && await userManager.CheckPasswordAsync(foundUser, userLoginDTO.Password))
                {
                    var roles = await userManager.GetRolesAsync(foundUser);
                    string userRole = roles.First().ToString();

                    var accessToken = GenerateTokens.GenerateAccessToken(foundUser, appSettings, userRole);
                    var refreshToken = await GenerateTokens.GenerateRefreshToken(foundUser, appSettings, appDbContext);
                    return Results.Ok(new { accessToken = accessToken, refreshToken = refreshToken });

                }
                else
                {
                    return Results.BadRequest(new { Message = "Invalid email or password." });
                }
            }
            catch (Exception ex)
            {
                // Send 500 internal server error with relevant exception data
                return Results.Problem(new
                {
                    Message = "An error occurred during registration.",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace // Only if you need to show stack trace in development
                }.ToString(), statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        private static async Task<IResult> RefreshToken(
            UserManager<AppUser> userManager,
            IOptions<AppSettings> appSettings,
            RefreshTokensDTO refreshTokensDTO,
            ApplicationDbContext appDbContext
            )
        {

            var refreshToken = refreshTokensDTO.RefreshToken;
            if (refreshToken == null)
            {
                return Results.BadRequest(new { Error = "Refresh Token is required" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var refreshSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.RefreshTokenSecret));

            try
            {
                // check token is exist in database in RefreshTokens table and it's property IsUsed is false, if not return 401
                // if it's exist, set IsUsed to true and save changes
                var foundRefreshToken = await appDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken && x.IsUsed == false);
                if (foundRefreshToken == null)
                {
                    return Results.Unauthorized();
                }

                // Validate the refresh token
                var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = refreshSecret,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var userId = principal.FindFirst("UserId")?.Value;

                if (userId == null)
                {
                    return Results.Unauthorized();
                }

                var foundUser = await userManager.FindByIdAsync(userId);
                if (foundUser == null)
                {
                    return Results.Unauthorized();
                }

                var roles = await userManager.GetRolesAsync(foundUser);
                string userRole = roles.First().ToString();

                // Set the refresh token to used
                foundRefreshToken.IsUsed = true;
                appDbContext.RefreshTokens.Update(foundRefreshToken);
                await appDbContext.SaveChangesAsync();

                // Create a new access token
                var newAccessToken = GenerateTokens.GenerateAccessToken(foundUser, appSettings, userRole);
                var newRefreshToken = await GenerateTokens.GenerateRefreshToken(foundUser, appSettings, appDbContext);

                return Results.Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
            }
            catch (Exception ex)
            {
                // send 500 internal server error
                var statusCode = (int)HttpStatusCode.InternalServerError;
                var error = JsonSerializer.Serialize(new { error = ex });
                return Results.Problem(error, statusCode.ToString());
            }
        }

    }
}
