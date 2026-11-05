namespace IceCreamShop.Middleware;

public class ShopClosedMiddleware
{
    private readonly RequestDelegate _next;

    public ShopClosedMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("ShopClosed"))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("The ice cream shop is closed. Please come back later.");
            return;
        }

        await _next(context);
    }
}

public static class ShopClosedMiddlewareExtensions
{
    public static IApplicationBuilder UseShopClosed(this IApplicationBuilder builder) =>
        builder.UseMiddleware<ShopClosedMiddleware>();
}
