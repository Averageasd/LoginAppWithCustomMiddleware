using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace LoginWithMiddleware.CustomMiddleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoginMiddleware
    {
        private readonly RequestDelegate _next;

        public LoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string method = httpContext.Request.Method; 
            if (method == "GET")
            {
                await httpContext.Response.WriteAsync("No response");
                return;
            }
            // Request Body is a stream 
            StreamReader streamReader = new StreamReader(httpContext.Request.Body);
            // read it into a string using streamReader
            string body = await streamReader.ReadToEndAsync();
            Dictionary<string, StringValues> keyValuePairs = QueryHelpers.ParseQuery(body);
            await HandleLogin(httpContext, keyValuePairs);
        }

        public async Task HandleLogin(HttpContext httpContext, Dictionary<string, StringValues> keyValuePairs)
        {
            
            bool hasEmail = keyValuePairs.ContainsKey("email");
            bool hasPassword = keyValuePairs.ContainsKey("password");
            if (!hasEmail || !hasPassword)
            {
                httpContext.Response.StatusCode = 400;
                if (!hasEmail)
                {
                    await httpContext.Response.WriteAsync("Invalid input for 'email'\n");
                }
                if (!hasPassword)
                {
                    await httpContext.Response.WriteAsync("Invalid input for 'password'\n");
                }
            }
            else
            {
                string email = keyValuePairs["email"][0];
                string password = keyValuePairs["password"][0];
                if (email != "admin@example.com" || password != "admin1234")
                {
                    httpContext.Response.StatusCode = 400;
                    await httpContext.Response.WriteAsync("Invalid login\n");
                }
                else
                {
                    await httpContext.Response.WriteAsync("Successful login\n");
                }
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LoginMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoginMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoginMiddleware>();
        }
    }
}
