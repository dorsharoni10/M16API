using System.Net;

namespace AlignAPI.Exceptions
{
    public class M16ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<M16ExceptionHandlerMiddleware> _logger;

        public M16ExceptionHandlerMiddleware(RequestDelegate next, ILogger<M16ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (InvalidAddressException ex)
            {
                _logger.LogError(ex, $"{DateTime.Now} - Failed to validate address.");
                await HandleExceptionAsync(httpContext, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (InvalidSaveException ex)
            {
                _logger.LogError(ex, $"{DateTime.Now} - Error occurred while adding mission to the database.");
                await HandleExceptionAsync(httpContext, HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (InvalidDateException ex)
            {
                _logger.LogError(ex, $"{DateTime.Now} - Format error occurred.");
                await HandleExceptionAsync(httpContext, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (InvalidArgumentException ex)
            {
                _logger.LogError(ex, $"{DateTime.Now} - Argument error occurred.");
                await HandleExceptionAsync(httpContext, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{DateTime.Now} - An unexpected error occurred.");
                await HandleExceptionAsync(httpContext, HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(new {StatusCode = context.Response.StatusCode, Message = message }.ToString());
        }
    }
}
