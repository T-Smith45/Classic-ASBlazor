using Simple_Razor_Components.Components;

namespace Simple_Razor_Components.Common;

public static class DefaultsApi
{
    public static void MapDefaultEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapFallback((HttpContext context) => {
            var path = context.Request.Path;
            context.Request.Path = new PathString("/not-found");
            return RazorComponent.For<NotFoundComponent>()
                .With(nf => nf.RequestedPath, path.Value)
                .WithStatusCode(404)
                .Result();
        });
    }
}