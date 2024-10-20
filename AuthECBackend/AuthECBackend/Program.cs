using AuthECBackend.Config;
using AuthECBackend.Data;
using AuthECBackend.Models.DTO;
using AuthECBackend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// services from the Identity package
builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// confugure user registration requirements
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
});

// adding db context, connection string is stored in appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevDbConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS policy configuration
app.UseCors(options => CorsOptionsConfig.ConfigureCorsPolicy(options));

app.UseAuthorization();

app.MapControllers();

app
    .MapGroup("/api")
    .MapIdentityApi<AppUser>();

app.MapPost("/api/signup", async (
    UserManager<AppUser> userManager,
    [FromBody] UserRegisterDTO userRegisterDTO
    ) =>
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
    });

app.Run();