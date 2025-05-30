using Microsoft.AspNetCore.Http.HttpResults;
using Simple_Razor_Components.Components;

namespace Simple_Razor_Components.Common;

public static class SampleEndpointsGroup
{
    public static void SampleEndpoints(this IEndpointRouteBuilder routeBuilder)
    {

        routeBuilder.MapGet("/", () => RazorComponent.For<Home>().Result());
        
        routeBuilder.MapGet("/sample", () => RazorComponent.For<WithInfo>()
            .With(tp => tp.Title,"Strongly Typed Title")
            .With(tp => tp.Info,"Strongly Typed Info")
            .Result()
        );

        routeBuilder.MapGet("/sample-weak", () 
            => new RazorComponentResult<WithInfo>(new { Title = "Weakly Typed Title",Info = "Weakly Typed Info"}));
        

        routeBuilder.MapFallback((HttpContext context) => {
            var path = context.Request.Path;
            context.Request.Path = new PathString("/not-found");
            return RazorComponent.For<NotFoundComponent>()
                .With(nf => nf.RequestedPath, path.Value)
                .WithStatusCode(404)
                .Result();
        });
    }
}