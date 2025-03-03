using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ParkingAPI.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                
            }
        await _next(context);
        }
    }
}