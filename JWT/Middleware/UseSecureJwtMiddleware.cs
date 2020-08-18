using Microsoft.AspNetCore.Builder;


namespace AuthReg.Middleware
{
    public static class UseSecureJwtMiddleware
    {
        public static IApplicationBuilder UseSecureJwt(this IApplicationBuilder builder) => builder.UseMiddleware<SecureJwtMiddleware>();
    }
}
