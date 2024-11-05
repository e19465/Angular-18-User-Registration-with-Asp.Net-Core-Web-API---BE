using System.Security.Claims;
using AuthECBackend.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AuthECBackend.Controllers
{
    public static class AccountEndPoints
    {
        public static IEndpointRouteBuilder MapAccountEndPoints(this IEndpointRouteBuilder app)
        {
            // GET user profile endpoint
            app.MapGet("/user-profile", GetUserProfile);

            return app;
        }

        private static async Task<IResult> GetUserProfile(ClaimsPrincipal user, UserManager<AppUser> userManager)
        {
            string userId = user.Claims.First(x => x.Type == "UserId").Value;
            var userDetails = await userManager.FindByIdAsync(userId);
            if (userDetails != null)
            {
                return Results.Ok(new
                {
                    Email = userDetails.Email,
                    FullName = userDetails.FullName
                });
            }
            else
            {
                return Results.NotFound(new { Message = "User not found." });
            }
        }
    }
}
