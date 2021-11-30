namespace CarsInfoApp.Middlewares
{
    using CarsInfoApp.Utils;
    using Microsoft.AspNetCore.Http;
    using System.Linq;
    using System.Threading.Tasks;

    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IJwtUtils jwtUtils)
        {
            var tokenFromCookie = context.Request.Cookies["token"]?.ToString().Split(" ").Last();
            var userId = jwtUtils.ValidateJwtToken(tokenFromCookie);
            if (userId != null)
            {
                // attach user to context on successful jwt validation
                context.Items["UserId"] = userId.Value;
            }

            await _next(context);
        }
    }
}
