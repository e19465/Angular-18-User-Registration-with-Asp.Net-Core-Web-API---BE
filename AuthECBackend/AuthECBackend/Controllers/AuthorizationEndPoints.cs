using Microsoft.AspNetCore.Authorization;

namespace AuthECBackend.Controllers
{
    public static class AuthorizationEndPoints
    {
        public static IEndpointRouteBuilder MapAuthorizationEndPoints(this IEndpointRouteBuilder app)
        {
            // Admin only example
            app.MapGet("/admin-only", AdminOnlyExample);
            app.MapGet("/admin-and-teacher-only", AdminAndTeacehrOnlyExample);
            app.MapGet("/teacher-only", TeacherOnlyExample);
            app.MapGet("/student-only", StudentOnlyExample);
            app.MapGet("/library-members-only", LibraryMembersOnly);
            app.MapGet("/apply-maternity-leave", ApplyMaternityLeave);
            app.MapGet("/under-10-females-only", UnderTenFemalesOnly);

            return app;
        }

        [Authorize(Roles = "Admin")]
        private static Task<IResult> AdminOnlyExample()
        {
            return Task.FromResult(Results.Ok(new { Message = "Admin Only" }));
        }


        [Authorize(Roles = "Admin,Teacher")]
        private static Task<IResult> AdminAndTeacehrOnlyExample()
        {
            return Task.FromResult(Results.Ok(new { Message = "Admin and Teacher only" }));
        }

        
        [Authorize(Roles = "Teacher")]
        private static Task<IResult> TeacherOnlyExample ()
        {
            return Task.FromResult(Results.Ok(new { Message = "Teacher only" }));
        }


        [Authorize(Roles = "Student")]
        private static Task<IResult> StudentOnlyExample()
        {
            return Task.FromResult(Results.Ok(new { Message = "Student only" }));
        }

        [Authorize(Policy = "HasLibraryId")]
        private static Task<IResult> LibraryMembersOnly()
        {
            return Task.FromResult(Results.Ok(new { Message = "Library Members only" }));
        }

        [Authorize(Roles = "Teacher", Policy = "FemalesOnly")]
        private static Task<IResult> ApplyMaternityLeave()
        {
            return Task.FromResult(Results.Ok(new { Message = "Female Teachers only" }));
        }

        [Authorize(Policy = "FemalesOnly")]
        [Authorize(Policy = "AgeUnderTenOnly")]
        private static Task<IResult> UnderTenFemalesOnly()
        {
            return Task.FromResult(Results.Ok(new { Message = "Under 10 Females only" }));
        }
    }
    
}
