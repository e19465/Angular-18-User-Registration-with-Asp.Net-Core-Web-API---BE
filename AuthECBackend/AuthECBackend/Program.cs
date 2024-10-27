using AuthECBackend.Controllers;
using AuthECBackend.Extensions;
using AuthECBackend.Models.Entities;

// Define the builder
var builder = WebApplication.CreateBuilder(args);

// add controllers
builder.Services.AddControllers();

// Injecting through extension methods
builder.Services.AddSwagger()
                .InjetcDbContext(builder.Configuration)
                .AddAppConfig(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.ConfigureSwagger()
   .ConfigureCORS(builder.Configuration)
   .AddIdentityAuthMiddlewares();

// Map the controllers
app.MapControllers();

// ** Map the endpoints ** //

// Identity endpoints
app.MapGroup("/api")
   .MapIdentityApi<AppUser>();

// User related endpoints
app.MapGroup("/api")
   .MapIdentityuserEndpoints();

// Run the app
app.Run();