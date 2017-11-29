using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace NancyMusicStore
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate next;
        public AuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public Task Invoke(HttpContext context) => context.ChallengeAsync();
    }
}