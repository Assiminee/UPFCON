using UPFCON.Exceptions;

namespace UPFCON.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private RequestDelegate Next { get; } = next;
    public ILogger<GlobalExceptionMiddleware> Logger { get; } = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await Next(context);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unhandled exception");

            var statusCode = ex switch
            {
                InvalidUserRoleException => StatusCodes.Status400BadRequest,
                InvalidLoginCredentialsException => StatusCodes.Status401Unauthorized,
                InvalidFileException => StatusCodes.Status400BadRequest,
                EmailNotConfirmedException => StatusCodes.Status401Unauthorized,
                ArgumentNullException => StatusCodes.Status400BadRequest,
                DuplicateEmailException => StatusCodes.Status409Conflict,
                NotFoundException => StatusCodes.Status404NotFound,
                ForbiddenException => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError,
            };
            
            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;
                
                Console.WriteLine("Status : " +statusCode + " || Error : " + ex.Message);

                var res = new
                {
                    status = statusCode,
                    message = ex.Message
                };

                await context.Response.WriteAsJsonAsync(res);
            }
        }
    }
}