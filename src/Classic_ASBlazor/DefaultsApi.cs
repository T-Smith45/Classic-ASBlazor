using Classic_ASBlazor.Common;
using Classic_ASBlazor.Components;

namespace Classic_ASBlazor;

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