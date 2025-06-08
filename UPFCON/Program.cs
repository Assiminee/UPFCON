using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UPFCON.Interfaces;
using UPFCON.Middlewares;
using UPFCON.Models;
using UPFCON.Models.Context;
using UPFCON.Services;
using UPFCON.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<UpfconContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        // Defining password requirements
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        
        // Setting max failed attempts and timespan until next possible attempt
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        
        // Requiring a unique email
        options.User.RequireUniqueEmail = true;
        
        // Requiring email confirmation
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<UpfconContext>()
    .AddDefaultTokenProviders();


// Explicitly forcing “every authentication action” to go through the JWT‐Bearer handler
builder.Services.AddAuthentication(options =>
    {
        // Means: "When the framework needs to validate a user’s identity on incoming requests,
        // use the JWT‐Bearer handler."
        options.DefaultAuthenticateScheme =
            // Means: “If an unauthenticated request hits an [Authorize] endpoint,
            // challenge by asking for a Bearer token.”
            options.DefaultChallengeScheme =
                // Means: “If an authenticated user lacks sufficient rights (e.g. wrong role) and we call Forbid(),
                // use the Bearer handler to produce a 403 response.”
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
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"] ?? string.Empty
            )),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SMTP"));
builder.Services.Configure<RegistrationEmailSettings>(builder.Configuration.GetSection("Emails:Registration"));
builder.Services.Configure<ActivateAccountSettings>(builder.Configuration.GetSection("Emails:AccountActivation"));
builder.Services.AddScoped<IDiplomaService, DiplomaService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IEmailSender, EmailSenderService>();
builder.Services.AddScoped<IUtils, Utils>();
builder.Services.AddScoped<IAuth, AuthService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

// Overrides ASP.NET's default validation (validation annotations of class properties)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors)
            .Select(e => e.ErrorMessage);

        var response = new
        {
            status = StatusCodes.Status400BadRequest,
            message = "Validation failed",
            errors
        };
        
        return new BadRequestObjectResult(response);
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using var scope = app.Services.CreateScope();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

var roles = Enum.GetNames<Roles>();

foreach (var roleName in roles)
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