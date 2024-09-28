using Microsoft.Data.SqlClient;
using RestApi.Src.CustomExceptions;

namespace RestApi.Src.Middlewares
{
    public class GlobalErrorHandling
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var req = context.Request;
            var res = context.Response;
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException)
            {
                res.StatusCode = 401;
                await res.WriteAsJsonAsync(string.Empty);
            }
            catch (ArgumentException) // curse when invalid jwt secret token provided or no secret token provided
            {
                res.StatusCode = 500;
                await res.WriteAsJsonAsync(
                    new { message = "something went wrong with json web token service" }
                );
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e);
                res.StatusCode = 500;
                await res.WriteAsJsonAsync(
                    new { message = "something went wrong with database connection" }
                );
            }
            catch (ValidationExceptions e)
            {
                res.StatusCode = 400;
                await res.WriteAsJsonAsync(
                    new { property = e.Errors[0].PropertyName, message = e.Errors[0].ErrorMessage }
                );
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                res.StatusCode = 500;
                await res.WriteAsJsonAsync(new { message = "something went wrong" });
            }
        }
    }
}
