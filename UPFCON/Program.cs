using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UPFCON.Models;
using UPFCON.Models.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<UpfconContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Define password requirements
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 10;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<UpfconContext>();

builder.Services.AddAuthorization();

// Explicitly forcing “every authentication action” to go through the JWT‐Bearer handler
builder.Services.AddAuthentication(options =>
    {
        // Means: "When the framework needs to validate a user’s identity on incoming requests, use the JWT‐Bearer handler."
        options.DefaultAuthenticateScheme =
            // Means: “If an unauthenticated request hits an [Authorize] endpoint, challenge by asking for a Bearer token.”
            options.DefaultChallengeScheme =
                // Means: “If an authenticated user lacks sufficient rights (e.g. wrong role) and we call Forbid(), use the Bearer handler to produce a 403 response.”
                options.DefaultForbidScheme =
                    options.DefaultScheme =
                        options.DefaultSignInScheme =
                            options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    // Configuring how the JWT Bearer authentication handler will validate incoming tokens
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Tells the API to validate the issuer of the token (our server in this case: localhost)
            // Any JWT presented with a different issuer (iss) claim will be rejected.
            ValidateIssuer = true,
            // In the appsettings.json, we define the issuer: http://localhost:5246
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            
            // Validates which API(s) this token is meant for
            // Forces the middleware to check that the token’s audience (aud)
            // claim matches exactly the one defined in appsettings.json (http://localhost:5246 in this case)
            ValidateAudience = true,
            // If the token’s aud claim does not match this value, it’s considered invalid.
            ValidAudience = builder.Configuration["JWT:Audience"],
            
            // A JWT is signed by the issuer to prove it wasn't tampered with.
            // By turning on ValidateIssuerSigningKey, we’re telling the framework:
            // “I require that the JWT’s signature be validated using the key defined in appsettings.json (or CLI arguments.)”
            ValidateIssuerSigningKey = true,
            // This is the actual key the server will use to verify the token’s HMAC signature.
            // We’re reading builder.Configuration["JWT:SigningKey"], converting that string
            
            // into a byte array, and wrapping it as a SymmetricSecurityKey.
            
            // If the JWT was signed with the same key (or the matching private key, in an asymmetric scenario),
            // the signature check passes. Otherwise, the token is rejected.
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]
                ))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using var scope = app.Services.CreateScope();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

string[] roles = new[] { "Admin", "Chairman", "Attendee", "Author", "BoardDirector" };

foreach (string roleName in roles)
{
    if (!await roleManager.RoleExistsAsync(roleName))
    {
        var role = new IdentityRole<Guid>(roleName);
        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                logger.LogError("Failed to create role {RoleName}: {ErrorCode} - {ErrorDesc}",
                    roleName, error.Code, error.Description);
            }
        } else {
            logger.LogInformation("Successfully created role {RoleName}", roleName);
        }
    } else {
        logger.LogInformation("Role {RoleName} already exists, skipping creation", roleName);
    }
}

app.Run();